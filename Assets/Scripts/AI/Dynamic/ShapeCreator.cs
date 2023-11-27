using System;
using UnityEngine;
using System.Collections.Generic;

public class ShapeCreator : MonoBehaviour
{
    [Serializable]
    public class ShapeData
    {
        public string type;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public List<Vector3> vertices;
        public List<int> triangles;
        public Color newColor;
    }

    [Serializable]
    public class ShapeDataList
    {
        public List<ShapeData> objects;
    }

    public GameObject CreateMeshFromJson(string jsonString)
    {
        ShapeDataList shapeDataList = JsonUtility.FromJson<ShapeDataList>(jsonString);

        if (shapeDataList != null && shapeDataList.objects != null && shapeDataList.objects.Count > 0)
        {
            ShapeData shapeData = shapeDataList.objects[0]; // Assuming only one shape for simplicity

            return CreateObject(shapeData, CreateMesh(shapeData));
        }
        else
        {
            Debug.LogError("Invalid shape data list. Please check the JSON structure.");
            return null;
        }
    }

    public Mesh CreateMesh(ShapeData shapeData)
    {
        Mesh mesh = new Mesh();
        Vector3[] scaledVertices = new Vector3[shapeData.vertices.Count];

        for (int i = 0; i < shapeData.vertices.Count; i++)
        {
            scaledVertices[i] = Vector3.Scale(shapeData.vertices[i], shapeData.scale);
        }

        mesh.vertices = scaledVertices;
        mesh.triangles = shapeData.triangles.ToArray();

        mesh.RecalculateNormals();

        for (int i = 0; i < mesh.normals.Length; i++)
        {
            mesh.normals[i] *= -1f;
        }

        mesh.RecalculateBounds();

        return mesh;
    }

    public GameObject CreateObject(ShapeData shapeData, Mesh mesh)
    {
        GameObject spawnedObject = new GameObject("SpawnedObject");
        MeshFilter meshFilter = spawnedObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = spawnedObject.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = spawnedObject.AddComponent<MeshCollider>();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.color = shapeData.newColor;

        meshRenderer.material = material;

        return spawnedObject;
    }
}