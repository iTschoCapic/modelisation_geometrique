using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class test : MonoBehaviour
{

    private void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        MeshLoader meshLoader = gameObject.AddComponent<MeshLoader>();
        meshLoader.LoadOff(".\\off\\buddha.off");

        mesh.vertices = meshLoader.meshData.vertices.ToArray();
        mesh.triangles = meshLoader.meshData.faces.ToArray();
        mesh.normals = meshLoader.meshData.normals.ToArray();

        // Afficher les données chargées dans la console
        Debug.Log("Sommets: " + meshLoader.meshData.vertices.Count);
        Debug.Log("Facettes: " + meshLoader.meshData.faces.Count);
        Debug.Log("Normales: " + meshLoader.meshData.normals.Count);
        meshLoader.RemoveFaces(5);
        meshLoader.ExportObj("Assets/modified_model.obj");
    }
}
