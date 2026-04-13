using UnityEngine;
using System.IO;

public class NeuralNetworkSaver
{
    public static void Save(NeuralNetwork brain, string filename)
    {
        BrainData data = new BrainData(brain);
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        string path = Path.Combine(Application.persistentDataPath, filename + ".json");
        File.WriteAllText(path, json);
        Debug.Log($"Brain saved to {path}");
    }

    public static void Load(NeuralNetwork brain, string filename)
    {
        string path = Path.Combine(Application.persistentDataPath, filename + ".json");
        if (!File.Exists(path)) { Debug.LogWarning("No save found."); return; }

        string json = File.ReadAllText(path);
        BrainData data = JsonUtility.FromJson<BrainData>(json);
        data.ApplyTo(brain);
        Debug.Log("Brain loaded!");
    }
}

[System.Serializable]
public class BrainData
{
    public float[] flatWeights;
    public float[] flatBiases;
    public int[] layerSizes;

    public BrainData(NeuralNetwork net)
    {
        // Flatten weights/biases into 1D arrays for JSON serialization
        layerSizes = net.layerSizes;
        flatWeights = net.GetFlatWeights();
        flatBiases = net.GetFlatBiases();
    }

    public void ApplyTo(NeuralNetwork net) =>
        net.SetWeightsAndBiases(flatWeights, flatBiases);
}

