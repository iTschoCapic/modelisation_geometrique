using UnityEngine;

public class CylinderGenerator : MonoBehaviour
{
    public float rayon = 1.0f;        // Rayon du cylindre
    public float hauteur = 2.0f;      // Hauteur du cylindre
    public int nb_Meridiens = 20;     // Nombre de méridiens

    void Start()
    {
        // Créer un nouveau Mesh
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Variables pour la création des sommets et des triangles
        Vector3[] vertices = new Vector3[2 * (nb_Meridiens + 1) + 2]; // Sommets pour le corps et les couvercles
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[nb_Meridiens * 12]; // 6 triangles par méridien (2 latéraux + 4 pour les couvercles)

        // Création des sommets pour le corps du cylindre
        int vertIndex = 0;
        float angleStep = 2 * Mathf.PI / nb_Meridiens;
        for (int i = 0; i <= nb_Meridiens; i++)
        {
            float angle = i * angleStep;
            float x = Mathf.Cos(angle) * rayon;
            float z = Mathf.Sin(angle) * rayon;

            // Sommet inférieur (corps)
            vertices[vertIndex] = new Vector3(x, 0, z);
            uv[vertIndex] = new Vector2((float)i / nb_Meridiens, 0);
            vertIndex++;

            // Sommet supérieur (corps)
            vertices[vertIndex] = new Vector3(x, hauteur, z);
            uv[vertIndex] = new Vector2((float)i / nb_Meridiens, 1);
            vertIndex++;
        }

        // Sommet central pour la base inférieure
        vertices[vertIndex] = new Vector3(0, 0, 0); // Centre bas
        uv[vertIndex] = new Vector2(0.5f, 0.5f);
        int baseInferieureCentreIndex = vertIndex;
        vertIndex++;

        // Sommet central pour la base supérieure
        vertices[vertIndex] = new Vector3(0, hauteur, 0); // Centre haut
        uv[vertIndex] = new Vector2(0.5f, 0.5f);
        int baseSuperieureCentreIndex = vertIndex;
        vertIndex++;

        // Création des triangles pour le corps du cylindre (ordre inversé pour corriger les normales)
        int triIndex = 0;
        for (int i = 0; i < nb_Meridiens; i++)
        {
            // Indices des sommets pour deux triangles latéraux
            int baseA = i * 2;
            int baseB = (i + 1) * 2;

            // Triangle inférieur (face latérale) - inversé pour corriger la normale
            AddTriangle(triangles, ref triIndex, baseA + 1, baseB, baseA);

            // Triangle supérieur (face latérale) - inversé pour corriger la normale
            AddTriangle(triangles, ref triIndex, baseA + 1, baseB + 1, baseB);
        }

        // Création des triangles pour le couvercle inférieur
        for (int i = 0; i < nb_Meridiens; i++)
        {
            int baseA = i * 2;
            int baseB = (i + 1) * 2;
            AddTriangle(triangles, ref triIndex, baseInferieureCentreIndex, baseA, baseB);
        }

        // Création des triangles pour le couvercle supérieur
        for (int i = 0; i < nb_Meridiens; i++)
        {
            int baseA = i * 2 + 1;
            int baseB = (i + 1) * 2 + 1;
            AddTriangle(triangles, ref triIndex, baseSuperieureCentreIndex, baseB, baseA);
        }

        // Affecter les vertices, les UV et les triangles au Mesh
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        // Recalculer les normales pour l'éclairage
        mesh.RecalculateNormals();
    }

    // Fonction pour ajouter un triangle
    void AddTriangle(int[] triangles, ref int triIndex, int a, int b, int c)
    {
        triangles[triIndex] = a;
        triangles[triIndex + 1] = b;
        triangles[triIndex + 2] = c;
        triIndex += 3;
    }
}
