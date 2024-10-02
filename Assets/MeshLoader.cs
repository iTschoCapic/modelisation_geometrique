using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Globalization;
using UnityEngine.TextCore.Text;
using UnityEditor.UI;
using UnityEngine.UIElements;

public class MeshLoader : MonoBehaviour
{
    public class MeshData
    {
        public List<Vector3> vertices = new List<Vector3>(); // Liste pour les sommets
        public List<int> faces = new List<int>(); // Liste pour les facettes
        public List<Vector3> normals = new List<Vector3>(); // Liste pour les normales
    }

    public MeshData meshData;

    public MeshLoader()
    {
        meshData = new MeshData();
    }

    public void LoadOff(string filePath)
    {
        using (StreamReader reader = new StreamReader(filePath))
        {
            // Lire la première ligne
            string line = reader.ReadLine().Trim();
            if (line != "OFF")
            {
                throw new Exception("Ce fichier n'est pas au format OFF");
            }

            // Lire les dimensions
            string[] dimensions = reader.ReadLine().Trim().Split(' ');
            int numVertices = int.Parse(dimensions[0]);
            int numFaces = int.Parse(dimensions[1]);

            // Lire les sommets
            for (int i = 0; i < numVertices; i++)
            {
                line = reader.ReadLine().Trim();
                string[] vertexData = line.Split(' ');
                Vector3 vertex = new Vector3(float.Parse(vertexData[0], CultureInfo.InvariantCulture), float.Parse(vertexData[1], CultureInfo.InvariantCulture), float.Parse(vertexData[2], CultureInfo.InvariantCulture));
                meshData.vertices.Add(vertex);
            }

            Vector3 diffCenter = Vector3.zero - centerOff();
            for (int i = 0; i < meshData.vertices.Count; i++)
            {
                meshData.vertices[i] += diffCenter;
            }

            // Lire les facettes
            for (int i = 0; i < numFaces; i++)
            {
                line = reader.ReadLine().Trim();
                string[] faceData = line.Split(' ');
                int numVerticesInFace = int.Parse(faceData[0]); // Toujours 3 pour des facettes triangulaires

                for (int j = 1; j <= numVerticesInFace; j++)
                {
                    meshData.faces.Add(int.Parse(faceData[j]));
                }
                
            }
            NormalizeVertices();
            NormalisationFaces();
        }
    }

    public void ExportObj(string exportFilePath)
    {
        using (StreamWriter writer = new StreamWriter(exportFilePath))
        {
            // Écrire les sommets (vertex)
            foreach (Vector3 vertex in meshData.vertices)
            {
                writer.WriteLine($"v {vertex.x.ToString(CultureInfo.InvariantCulture)} {vertex.y.ToString(CultureInfo.InvariantCulture)} {vertex.z.ToString(CultureInfo.InvariantCulture)}");
            }

            // Écrire les normales (si elles existent)
            if (meshData.normals.Count > 0)
            {
                foreach (Vector3 normal in meshData.normals)
                {
                    writer.WriteLine($"vn {normal.x.ToString(CultureInfo.InvariantCulture)} {normal.y.ToString(CultureInfo.InvariantCulture)} {normal.z.ToString(CultureInfo.InvariantCulture)}");
                }
            }

            // Écrire les faces (triangles)
            for (int i = 0; i < meshData.faces.Count; i += 3)
            {
                int v1 = meshData.faces[i] + 1;      // Les indices commencent à 1 dans les fichiers .obj
                int v2 = meshData.faces[i + 1] + 1;
                int v3 = meshData.faces[i + 2] + 1;

                if (meshData.normals.Count > 0)
                {
                    // Si les normales existent, on ajoute les normales dans les faces
                    writer.WriteLine($"f {v1}//{v1} {v2}//{v2} {v3}//{v3}");
                }
                else
                {
                    // Sinon, on écrit juste les indices des sommets
                    writer.WriteLine($"f {v1} {v2} {v3}");
                }
            }
        }

        Debug.Log($"Maillage exporté avec succès vers {exportFilePath}");
    }


    private Vector3 centerOff()
    {
        Vector3 center = new Vector3();
        foreach (var vertice in meshData.vertices)
        {
            center += vertice;
        }
        return center / meshData.vertices.Count;
    }

    private void NormalizeVertices()
    {
        float maxAbsValue = 0f;

        // Trouver la plus grande valeur absolue parmi les coordonnées
        foreach (var vertex in meshData.vertices)
        {
            maxAbsValue = Mathf.Max(maxAbsValue, Mathf.Abs(vertex.x), Mathf.Abs(vertex.y), Mathf.Abs(vertex.z));
        }

        // Normaliser les coordonnées
        if (maxAbsValue > 0f) // Éviter la division par zéro
        {
            for (int i = 0; i < meshData.vertices.Count; i++)
            {
                meshData.vertices[i] /= maxAbsValue;
            }
        }
    }

    private void NormalisationFaces() // Produit scalaire de deux vecteurs
    {
        List<Vector3> vertexNormals = new List<Vector3>(new Vector3[meshData.vertices.Count]);
        for (int i = 0; i < meshData.faces.Count; i += 3) // Change to Vertices
        {
            Vector3 v0 = meshData.vertices[meshData.faces[i]];
            Vector3 v1 = meshData.vertices[meshData.faces[i + 1]];
            Vector3 v2 = meshData.vertices[meshData.faces[i + 2]];

            Vector3 diffV1 = v1 + (Vector3.zero - v0);
            Vector3 diffV2 = v2 + (Vector3.zero - v0);

            Vector3 produit = Vector3.Cross(diffV2, diffV1).normalized;

            // Ajouter cette normale à chaque sommet du triangle
            vertexNormals[meshData.faces[i]] += produit;
            vertexNormals[meshData.faces[i + 1]] += produit;
            vertexNormals[meshData.faces[i + 2]] += produit;

        }

        for (int i = 0; i < vertexNormals.Count; i++) // Mon problème venait d'ici, j'ajoutais meshData.normals.Add(produit) à l'indice i ligne 123, cependant i changeait de 3 en 3 donc l'indice final était 3x plus grand que cela n'aurait dûe! les indices était 0, 3, 6, 9 alors qu'ils auraient dûe être 0, 1, 2, 3.
        {
            meshData.normals.Add(vertexNormals[i]);
        }
    }

    public void RemoveFaces(int n)
    {
        if (n <= 0 || n >= meshData.faces.Count / 3)
        {
            throw new ArgumentException("Nombre de faces à supprimer invalide.");
        }

        // Créer une liste pour marquer les sommets utilisés
        HashSet<int> verticesUsed = new HashSet<int>();

        // On va retirer aléatoirement 'n' faces
        System.Random random = new System.Random();
        List<int> faceIndicesToRemove = new List<int>();

        // Ajouter aléatoirement des indices de faces à supprimer
        while (faceIndicesToRemove.Count < n)
        {
            int randomFaceIndex = random.Next(0, meshData.faces.Count / 3); // Chaque face a 3 sommets
            if (!faceIndicesToRemove.Contains(randomFaceIndex))
            {
                faceIndicesToRemove.Add(randomFaceIndex);
            }
        }

        // Créer de nouvelles listes pour stocker les faces et sommets restants
        List<int> newFaces = new List<int>();
        List<Vector3> newVertices = new List<Vector3>();

        // Utiliser un dictionnaire pour mapper les anciens indices des sommets aux nouveaux
        Dictionary<int, int> oldToNewIndexMap = new Dictionary<int, int>();

        // Ajouter les faces restantes et mapper les nouveaux indices des sommets
        for (int i = 0; i < meshData.faces.Count; i += 3)
        {
            int faceIndex = i / 3; // Indice de la face
            if (faceIndicesToRemove.Contains(faceIndex)) continue; // Ne pas inclure cette face

            for (int j = 0; j < 3; j++)
            {
                int oldIndex = meshData.faces[i + j];
                if (!oldToNewIndexMap.ContainsKey(oldIndex))
                {
                    // Ajouter le sommet au nouveau tableau et mapper son nouvel index
                    oldToNewIndexMap[oldIndex] = newVertices.Count;
                    newVertices.Add(meshData.vertices[oldIndex]);
                }
                // Ajouter le nouvel indice de sommet dans les faces
                newFaces.Add(oldToNewIndexMap[oldIndex]);
            }
        }

        // Mettre à jour les données du maillage
        meshData.vertices = newVertices;
        meshData.faces = newFaces;
    }

}
