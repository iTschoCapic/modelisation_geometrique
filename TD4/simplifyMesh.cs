using System.Collections.Generic;
using UnityEngine;

public class simplifyMesh : MonoBehaviour
{
    public float cellSize = 1f;

    private void Start()
    {
        SimplifyMesh();
    }

    public void SimplifyMesh()
    {
        // Initialisation
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        // Dictionary to store vertices grouped by their cell coordinates
        // Key: cell coordinate (Vector3Int), Value: List of vertex indices in that cell
        Dictionary<Vector3Int, List<int>> cellDictionary = new Dictionary<Vector3Int, List<int>>();

        // Loop through all the vertices to group them into cells
        for (int i = 0; i < vertices.Length; i++)
        {
            // Determine which cell the current vertex belongs to based on its position
            Vector3Int cell = GetCellCoordinate(vertices[i]);

            // If the cell does not exist in the dictionary, create a new list for its vertices
            if (!cellDictionary.ContainsKey(cell))
            {
                cellDictionary[cell] = new List<int>();
            }

            // Add the index of the vertex to the corresponding cell's list
            cellDictionary[cell].Add(i);
        }

        // Array to store the new vertices after simplification
        Vector3[] newVertices = new Vector3[cellDictionary.Count];

        // Array to map the old vertex indices to the new ones (for updating the triangles)
        int[] oldToNewMap = new int[vertices.Length];

        // Counter to track the index of new vertices
        int newIndex = 0;

        // Loop through each cell in the dictionary and calculate the new vertex positions
        foreach (var cell in cellDictionary)
        {
            // Variable to store the summed position of all vertices in the current cell
            Vector3 mergedPosition = Vector3.zero;

            // Loop through all vertices in the current cell and sum their positions
            foreach (int vertexIndex in cell.Value)
            {
                mergedPosition += vertices[vertexIndex];
            }

            // Calculate the average position by dividing by the number of vertices in the cell
            mergedPosition /= cell.Value.Count;

            // Loop through all vertices in the current cell and update their old-to-new index mapping
            foreach (int vertexIndex in cell.Value)
            {
                oldToNewMap[vertexIndex] = newIndex;
            }

            // Store the averaged position as the new vertex
            newVertices[newIndex] = mergedPosition;
            newIndex++;
        }

        int[] newTriangles = new int[triangles.Length];

        // Loop through all triangles and update their vertex indices using the old-to-new map
        for (int i = 0; i < triangles.Length; i++)
        {
            newTriangles[i] = oldToNewMap[triangles[i]];
        }

        mesh.Clear();

        // Set the new simplified vertices and triangles to the mesh
        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;

        // Recalculate the normals of the mesh to update the lighting and shading
        mesh.RecalculateNormals();
    }
    private Vector3Int GetCellCoordinate(Vector3 position)
    {
        return new Vector3Int(
            Mathf.FloorToInt(position.x / cellSize),
            Mathf.FloorToInt(position.y / cellSize),
            Mathf.FloorToInt(position.z / cellSize)
        );
    }
}
