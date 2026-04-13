using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class NeuralNetwork
{
    private float[][] neurons;      // neuron values per layer
    private float[][][] weights;    // weights[layer][from][to]
    private float[][] biases;       // biases[layer][neuron]
    public int[] layerSizes;

    public NeuralNetwork(int[] layerSizes)
    {
        this.layerSizes = layerSizes;
        neurons = new float[layerSizes.Length][];
        weights = new float[layerSizes.Length - 1][][];
        biases = new float[layerSizes.Length][];

        // Initialize neurons and biases
        for (int i = 0; i < layerSizes.Length; i++)
        {
            neurons[i] = new float[layerSizes[i]];
            biases[i] = new float[layerSizes[i]];
        }

        // Initialize weights with small random values (Xavier initialization)
        for (int i = 0; i < layerSizes.Length - 1; i++)
        {
            weights[i] = new float[layerSizes[i]][];
            float scale = Mathf.Sqrt(2.0f / (layerSizes[i] + layerSizes[i + 1]));

            for (int j = 0; j < layerSizes[i]; j++)
            {
                weights[i][j] = new float[layerSizes[i + 1]];
                for (int k = 0; k < layerSizes[i + 1]; k++)
                {
                    // Xavier: keeps signals from vanishing or exploding
                    weights[i][j][k] = (float)(UnityEngine.Random.value * 2 - 1) * scale;
                }
            }
        }
    }

    // Forward pass: turn state input into action probabilities
    public float[] FeedForward(float[] inputs)
    {
        neurons[0] = inputs;

        for (int layer = 1; layer < layerSizes.Length; layer++)
        {
            for (int j = 0; j < layerSizes[layer]; j++)
            {
                float sum = biases[layer][j];
                for (int i = 0; i < layerSizes[layer - 1]; i++)
                    sum += neurons[layer - 1][i] * weights[layer - 1][i][j];

                // ReLU for hidden layers, Softmax handled after for output
                neurons[layer][j] = layer < layerSizes.Length - 1
                    ? ReLU(sum)
                    : sum; // raw logits, softmax applied separately
            }
        }

        return Softmax(neurons[layerSizes.Length - 1]);
    }

    private float ReLU(float x) => Mathf.Max(0f, x);

    private float[] Softmax(float[] logits)
    {
        float max = logits.Max(); // stability trick
        float[] exps = logits.Select(x => Mathf.Exp(x - max)).ToArray();
        float sum = exps.Sum();
        return exps.Select(e => e / sum).ToArray();
    }



    // Flatten all weights into a single 1D array for JSON saving
    public float[] GetFlatWeights()
    {
        List<float> flat = new List<float>();
        for (int l = 0; l < weights.Length; l++)
            for (int i = 0; i < weights[l].Length; i++)
                for (int j = 0; j < weights[l][i].Length; j++)
                    flat.Add(weights[l][i][j]);
        return flat.ToArray();
    }


    // Flatten all biases into a single 1D array for JSON saving
    public float[] GetFlatBiases()
    {
        List<float> flat = new List<float>();
        for (int l = 0; l < biases.Length; l++)
            for (int j = 0; j < biases[l].Length; j++)
                flat.Add(biases[l][j]);
        return flat.ToArray();
    }


    // Restore weights and biases from flat arrays (used when loading)
    public void SetWeightsAndBiases(float[] flatWeights, float[] flatBiases)
    {
        // Restore weights
        int wi = 0; // index into flatWeights
        for (int l = 0; l < weights.Length; l++)
            for (int i = 0; i < weights[l].Length; i++)
                for (int j = 0; j < weights[l][i].Length; j++)
                    weights[l][i][j] = flatWeights[wi++];

        // Restore biases
        int bi = 0; // index into flatBiases
        for (int l = 0; l < biases.Length; l++)
            for (int j = 0; j < biases[l].Length; j++)
                biases[l][j] = flatBiases[bi++];
    }
}
