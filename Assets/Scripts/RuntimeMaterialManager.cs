using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class MaterialData
{
    public float[] color = new float[4];
    public string name;
    public float metallic;
    public float smoothness;

    public MaterialData(Material mat)
    {
        Color matColor = mat.color;
        color[0] = matColor.r;
        color[1] = matColor.g;
        color[2] = matColor.b;
        color[3] = matColor.a;
        name = mat.name;
        metallic = mat.GetFloat("_Metallic");
        smoothness = mat.GetFloat("_Smoothness");
    }
}

[System.Serializable]
public class MaterialCollection
{
    public List<MaterialData> materials = new List<MaterialData>();
}

public class RuntimeMaterialManager : MonoBehaviour
{
    private string saveFileName = "MaterialData.json";

    // Save in Documents folder
    private string SavePath => Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), saveFileName);

    [HideInInspector]
    public List<Material> generatedMaterials = new List<Material>();

    void Start()
    {
        // Debug.Log("Starting material manager...");
        // LoadMaterials();
    }

    public void GenerateNewMaterials()
    {
        Debug.Log("Generating new materials...");

        generatedMaterials.Clear();
        MaterialCollection collection = new MaterialCollection();

        for (int i = 0; i < 5; i++)
        {
            Material newMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            newMat.name = $"RuntimeMaterial_{i}";
            newMat.color = new Color(Random.value, Random.value, Random.value);
            newMat.SetFloat("_Metallic", Random.value);
            newMat.SetFloat("_Smoothness", Random.value);

            generatedMaterials.Add(newMat);
            collection.materials.Add(new MaterialData(newMat));

            CreateExampleSphere(newMat, i);
        }

        SaveMaterials(collection);
        Debug.Log("5new materials generated and saved.");
    }

    public void LoadMaterials()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("No saved materials found. Generating new ones...");
            GenerateNewMaterials();
            return;
        }

        try
        {
            Debug.Log($"Loading materials from: {SavePath}");

            string jsonData = File.ReadAllText(SavePath);
            MaterialCollection collection = JsonUtility.FromJson<MaterialCollection>(jsonData);

            generatedMaterials.Clear();

            foreach (MaterialData matData in collection.materials)
            {
                Material loadedMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                loadedMat.name = matData.name;
                loadedMat.color = new Color(matData.color[0], matData.color[1], matData.color[2], matData.color[3]);
                loadedMat.SetFloat("_Metallic", matData.metallic);
                loadedMat.SetFloat("_Smoothness", matData.smoothness);

                generatedMaterials.Add(loadedMat);
            }

            Debug.Log($"Loaded {generatedMaterials.Count} materials successfully.");

            // for (int i = 0; i < generatedMaterials.Count; i++)
            // {
            //     CreateExampleSphere(generatedMaterials[i], i);
            // }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load materials: {e.Message}");
        }
    }

    private void SaveMaterials(MaterialCollection collection)
    {
        try
        {
            string jsonData = JsonUtility.ToJson(collection, true);
            File.WriteAllText(SavePath, jsonData);
            Debug.Log($"Materials successfully saved to: {SavePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save materials: {e.Message}");
        }
    }

    private void CreateExampleSphere(Material mat, int index)
    {
        Debug.Log($"Creating sphere for material: {mat.name}");

        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.name = $"Sphere_{mat.name}";

        // Adjust position: reduce spacing and raise it
        float spacing = 1.2f;  // Reduce distance between spheres
        float height = 1.5f;   // Raise above the ground

        obj.transform.position = new Vector3(index * spacing, height, 0);
        obj.GetComponent<Renderer>().material = mat;

        // Ensure the sphere is properly visible in the scene
        obj.transform.SetParent(null);
    }


    public Material GetMaterial(int index)
    {
        if (index >= 0 && index < generatedMaterials.Count)
        {
            return generatedMaterials[index];
        }
        return null;
    }

    public List<Material> GetAllMaterials()
    {
        return new List<Material>(generatedMaterials);
    }


}
