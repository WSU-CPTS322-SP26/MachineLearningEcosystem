using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SharedBrain
{
    // --- The one shared network ---
    private NeuralNetwork actor;
    private NeuralNetwork critic;

    // --- One shared experience buffer for ALL creatures ---
    private List<Experience> experienceBuffer = new();

    // --- PPO Hyperparameters ---
    private const float LearningRate = 3e-4f;
    private const float Gamma = 0.99f;
    private const float Lambda = 0.95f;
    private const float ClipEpsilon = 0.2f;
    private const float ValueCoeff = 0.5f;
    private const float EntropyCoeff = 0.01f;
    private const int BatchSize = 64;
    private const int UpdateEpochs = 4;

    // --- Elite tracking ---
    // Experiences tagged with which creature contributed them
    private Dictionary<int, float> creatureTotalRewards = new();
    private int eliteCreatureId = -1;

    public SharedBrain(int[] layerSizes)
    {
        actor = new NeuralNetwork(layerSizes);

        int[] criticSizes = new int[layerSizes.Length];
        layerSizes.CopyTo(criticSizes, 0);
        criticSizes[criticSizes.Length - 1] = 1;
        critic = new NeuralNetwork(criticSizes);
    }

    // --- Called by each creature every step ---
    public (int action, float logProb, float value) SelectAction(float[] state)
    {
        float[] actionProbs = actor.FeedForward(state);
        float value = critic.FeedForward(state)[0];
        int action = SampleFromDistribution(actionProbs);
        float logProb = Mathf.Log(actionProbs[action] + 1e-8f);
        return (action, logProb, value);
    }

    // --- Store experience tagged with which creature it came from ---
    public void StoreExperience(Experience exp, int creatureId, float reward)
    {
        experienceBuffer.Add(exp);

        // Accumulate total reward per creature so we can find the elite
        if (!creatureTotalRewards.ContainsKey(creatureId))
            creatureTotalRewards[creatureId] = 0f;
        creatureTotalRewards[creatureId] += reward;
    }

    // --- Trigger the learning update ---
    // Called by SimulationManager every N steps
    public void Update()
    {
        if (experienceBuffer.Count < BatchSize) return;

        // Find the most successful creature this cycle
        if (creatureTotalRewards.Count > 0)
        {
            eliteCreatureId = creatureTotalRewards
                .OrderByDescending(kv => kv.Value)
                .First().Key;

            Debug.Log($"Elite creature this cycle: ID {eliteCreatureId} " +
                      $"with reward {creatureTotalRewards[eliteCreatureId]:F2}");
        }

        var (returns, advantages) = ComputeGAE();

        for (int epoch = 0; epoch < UpdateEpochs; epoch++)
        {
            int[] indices = Enumerable.Range(0, experienceBuffer.Count)
                                      .OrderBy(_ => UnityEngine.Random.value)
                                      .ToArray();

            for (int start = 0; start < indices.Length; start += BatchSize)
            {
                int[] batch = indices.Skip(start).Take(BatchSize).ToArray();
                UpdateMiniBatch(batch, returns, advantages);
            }
        }

        // Clear for next cycle
        experienceBuffer.Clear();
        creatureTotalRewards.Clear();
    }

    private (float[] returns, float[] advantages) ComputeGAE()
    {
        int n = experienceBuffer.Count;
        float[] returns = new float[n];
        float[] advantages = new float[n];
        float gae = 0f;

        for (int t = n - 1; t >= 0; t--)
        {
            float reward = experienceBuffer[t].Reward;
            float value = experienceBuffer[t].ValueEstimate;
            bool done = experienceBuffer[t].Done;
            float nextVal = done ? 0f : (t < n - 1
                ? experienceBuffer[t + 1].ValueEstimate
                : 0f);

            float delta = reward + Gamma * nextVal - value;
            gae = delta + Gamma * Lambda * (done ? 0f : gae);

            advantages[t] = gae;
            returns[t] = gae + value;
        }

        // Normalize advantages
        float mean = advantages.Average();
        float std = Mathf.Sqrt(advantages
                        .Select(a => (a - mean) * (a - mean))
                        .Average());
        for (int i = 0; i < n; i++)
            advantages[i] = (advantages[i] - mean) / (std + 1e-8f);

        return (returns, advantages);
    }

    private void UpdateMiniBatch(int[] batch, float[] returns, float[] advantages)
    {
        foreach (int i in batch)
        {
            Experience exp = experienceBuffer[i];

            float[] newProbs = actor.FeedForward(exp.State);
            float newLogProb = Mathf.Log(newProbs[exp.ActionTaken] + 1e-8f);
            float newValue = critic.FeedForward(exp.State)[0];

            float ratio = Mathf.Exp(newLogProb - exp.LogProbability);
            float advantage = advantages[i];
            float unclipped = ratio * advantage;
            float clipped = Mathf.Clamp(ratio,
                                   1f - ClipEpsilon,
                                   1f + ClipEpsilon) * advantage;

            // --- Elite scaling ---
            // Experiences from the elite creature get 2x weight
            // This biases learning toward the most successful behavior
            float eliteScale = (exp.CreatureId == eliteCreatureId) ? 2.0f : 1.0f;

            float policyLoss = -Mathf.Min(unclipped, clipped) * eliteScale;
            float valueLoss = ValueCoeff * Mathf.Pow(newValue - returns[i], 2);
            float entropy = -newProbs.Sum(p => p > 0 ? p * Mathf.Log(p) : 0f);
            float totalLoss = policyLoss + valueLoss - EntropyCoeff * entropy;

            actor.Backpropagate(exp.State, exp.ActionTaken, totalLoss, LearningRate);
            critic.Backpropagate(exp.State, 0, valueLoss, LearningRate);
        }
    }

    private int SampleFromDistribution(float[] probs)
    {
        float r = UnityEngine.Random.value;
        float cumulative = 0f;
        for (int i = 0; i < probs.Length; i++)
        {
            cumulative += probs[i];
            if (r <= cumulative) return i;
        }
        return probs.Length - 1;
    }

    public NeuralNetwork GetActor() => actor;
    public NeuralNetwork GetCritic() => critic;
}
