using UnityEngine;

public class CylinderGenerator : MonoBehaviour
{
    public float rayon = 1.0f;        // Rayon du cylindre
    public float hauteur = 2.0f;      // Hauteur du cylindre
    public int nb_Meridiens = 20;     // Nombre de m�ridiens

    void Start()
    {
        // Cr�er un nouveau Mesh
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Variables pour la cr�ation des sommets et des triangles
        Vector3[] vertices = new Vector3[2 * (nb_Meridiens + 1) + 2]; // Sommets pour le corps et les couvercles
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[nb_Meridiens * 12]; // 6 triangles par m�ridien (2 lat�raux + 4 pour les couvercles)

        // Cr�ation des sommets pour le corps du cylindre
        int vertIndex = 0;
        float angleStep = 2 * Mathf.PI / nb_Meridiens;
        for (int i = 0; i <= nb_Meridiens; i++)
        {
            float angle = i * angleStep;
            float x = Mathf.Cos(angle) * rayon;
            float z = Mathf.Sin(angle) * rayon;

            // Sommet inf�rieur (corps)
            vertices[vertIndex] = new Vector3(x, 0, z);
            uv[vertIndex] = new Vector2((float)i / nb_Meridiens, 0);
            vertIndex++;

            // Sommet sup�rieur (corps)
            vertices[vertIndex] = new Vector3(x, hauteur, z);
            uv[vertIndex] = new Vector2((float)i / nb_Meridiens, 1);
            vertIndex++;
        }

        // Sommet central pour la base inf�rieure
        vertices[vertIndex] = new Vector3(0, 0, 0); // Centre bas
        uv[vertIndex] = new Vector2(0.5f, 0.5f);
        int baseInferieureCentreIndex = vertIndex;
        vertIndex++;

        // Sommet central pour la base sup�rieure
        vertices[vertIndex] = new Vector3(0, hauteur, 0); // Centre haut
        uv[vertIndex] = new Vector2(0.5f, 0.5f);
        int baseSuperieureCentreIndex = vertIndex;
        vertIndex++;

        // Cr�ation des triangles pour le corps du cylindre (ordre invers� pour corriger les normales)
        int triIndex = 0;
        for (int i = 0; i < nb_Meridiens; i++)
        {
            // Indices des sommets pour deux triangles lat�raux
            int baseA = i * 2;
            int baseB = (i + 1) * 2;

            // Triangle inf�rieur (face lat�rale) - invers� pour corriger la normale
            AddTriangle(triangles, ref triIndex, baseA + 1, baseB, baseA);

            // Triangle sup�rieur (face lat�rale) - invers� pour corriger la normale
            AddTriangle(triangles, ref triIndex, baseA + 1, baseB + 1, baseB);
        }

        // Cr�ation des triangles pour le couvercle inf�rieur
        for (int i = 0; i < nb_Meridiens; i++)
        {
            int baseA = i * 2;
            int baseB = (i + 1) * 2;
            AddTriangle(triangles, ref triIndex, baseInferieureCentreIndex, baseA, baseB);
        }

        // Cr�ation des triangles pour le couvercle sup�rieur
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

        // Recalculer les normales pour l'�clairage
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
