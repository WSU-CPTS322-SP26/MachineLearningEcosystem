using System.Collections.Generic;
using System.Linq;
using UnityEngine;




public struct Experience
{
    public float[] State;        // what the creature saw
    public int ActionTaken;      // what it decided to do
    public float Reward;         // what reward it got
    public float[] NextState;    // what it saw after acting
    public bool Done;            // did the episode end?

    // For PPO specifically, we also store:
    public float LogProbability; // log prob of the action taken
    public float ValueEstimate;  // critic's value prediction
}



public class PPOAgent : MonoBehaviour
{
    private NeuralNetwork actor;
    private NeuralNetwork critic;
    private List<Experience> experienceBuffer = new();

    // PPO Hyperparameters
    private const float LearningRate = 3e-4f;
    private const float Gamma = 0.99f;   // discount factor
    private const float Lambda = 0.95f;   // GAE smoothing
    private const float ClipEpsilon = 0.2f;    // PPO clip range
    private const float ValueCoeff = 0.5f;    // critic loss weight
    private const float EntropyCoeff = 0.01f;   // exploration bonus
    private const int BatchSize = 64;
    private const int UpdateEpochs = 4;       // reuse each batch 4x

    // --- Step 1: Choose an action ---
    public (int action, float logProb, float value) SelectAction(float[] state)
    {
        float[] actionProbs = actor.FeedForward(state);
        float value = critic.FeedForward(state)[0];

        // Sample from probability distribution (not just take the max)
        int action = SampleFromDistribution(actionProbs);
        float logProb = Mathf.Log(actionProbs[action] + 1e-8f);

        return (action, logProb, value);
    }

    // --- Step 2: Store experience ---
    public void StoreExperience(Experience exp) => experienceBuffer.Add(exp);

    // --- Step 3: Compute returns using GAE (Generalized Advantage Estimation) ---
    // GAE smoothly blends short-term and long-term reward estimates
    private (float[] returns, float[] advantages) ComputeGAE()
    {
        int n = experienceBuffer.Count;
        float[] returns = new float[n];
        float[] advantages = new float[n];

        float gae = 0f;
        float nextValue = 0f;

        for (int t = n - 1; t >= 0; t--)
        {
            float reward = experienceBuffer[t].Reward;
            float value = experienceBuffer[t].ValueEstimate;
            bool done = experienceBuffer[t].Done;
            float nextVal = done ? 0f : (t < n - 1
                ? experienceBuffer[t + 1].ValueEstimate
                : nextValue);

            // TD error: immediate reward + discounted future value - current estimate
            float delta = reward + Gamma * nextVal - value;

            // GAE accumulates discounted TD errors
            gae = delta + Gamma * Lambda * (done ? 0f : gae);

            advantages[t] = gae;
            returns[t] = gae + value;
        }

        // Normalize advantages — very important for stable training!
        float mean = advantages.Average();
        float std = Mathf.Sqrt(advantages.Select(a => (a - mean) * (a - mean)).Average());
        for (int i = 0; i < n; i++)
            advantages[i] = (advantages[i] - mean) / (std + 1e-8f);

        return (returns, advantages);
    }

    // --- Step 4: The PPO Update ---
    public void Update()
    {
        if (experienceBuffer.Count < BatchSize) return;

        var (returns, advantages) = ComputeGAE();

        for (int epoch = 0; epoch < UpdateEpochs; epoch++)
        {
            // Shuffle and create mini-batches
            var indices = Enumerable.Range(0, experienceBuffer.Count)
                                    .OrderBy(_ => UnityEngine.Random.value)
                                    .ToArray();

            for (int start = 0; start < indices.Length; start += BatchSize)
            {
                int[] batch = indices.Skip(start).Take(BatchSize).ToArray();
                UpdateMiniBatch(batch, returns, advantages);
            }
        }

        experienceBuffer.Clear();
    }

    private void UpdateMiniBatch(int[] batch, float[] returns, float[] advantages)
    {
        foreach (int i in batch)
        {
            Experience exp = experienceBuffer[i];

            // Current action probabilities
            float[] newProbs = actor.FeedForward(exp.State);
            float newLogProb = Mathf.Log(newProbs[exp.ActionTaken] + 1e-8f);
            float newValue = critic.FeedForward(exp.State)[0];

            // Probability ratio: how much has the policy changed?
            float ratio = Mathf.Exp(newLogProb - exp.LogProbability);

            // PPO Clipped Objective — the core of PPO
            // Prevents the network from updating too aggressively
            float advantage = advantages[i];
            float unclipped = ratio * advantage;
            float clipped = Mathf.Clamp(ratio, 1f - ClipEpsilon,
                                                         1f + ClipEpsilon) * advantage;
            float policyLoss = -Mathf.Min(unclipped, clipped);

            // Critic loss: how wrong was the value estimate?
            float valueLoss = ValueCoeff * Mathf.Pow(newValue - returns[i], 2);

            // Entropy bonus: reward uncertainty to maintain exploration
            float entropy = -newProbs.Sum(p => p > 0 ? p * Mathf.Log(p) : 0f);
            float entropyBonus = EntropyCoeff * entropy;

            // Total loss
            float totalLoss = policyLoss + valueLoss - entropyBonus;

            // Backpropagate
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
}
