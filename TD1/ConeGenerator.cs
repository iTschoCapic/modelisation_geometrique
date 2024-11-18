using UnityEngine;

[RequireComponent(typeof(MeshFilter))]

public class ConeGenerator : MonoBehaviour
{
    public float rayon = 1.0f;           // Rayon de la base
    public float hauteur = 2.0f;         // Hauteur du cône
    public int nbMeridiens = 20;         // Nombre de méridiens (longitudes)

    void Start()
    {
        // Créer un nouveau Mesh
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Calculer le nombre de sommets
        int nbSommets = nbMeridiens + 2; // 1 sommet pour le haut + 1 pour le centre de la base
        Vector3[] vertices = new Vector3[nbSommets + 1]; // +1 pour le sommet du cône
        Vector2[] uv = new Vector2[nbSommets + 1];
        int[] triangles = new int[nbMeridiens * 3 + nbMeridiens * 3]; // Triangles pour le cône et la base

        // Créer le sommet supérieur
        vertices[0] = new Vector3(0, hauteur, 0); // Sommet supérieur
        uv[0] = new Vector2(0.5f, 1f); // UV du sommet supérieur

        // Créer le sommet central de la base
        vertices[nbMeridiens + 1] = new Vector3(0, 0, 0); // Sommet central de la base
        uv[nbMeridiens + 1] = new Vector2(0.5f, 0f); // UV du sommet central

        // Créer les sommets de la base
        for (int j = 0; j < nbMeridiens; j++)
        {
            float angle = 2 * Mathf.PI * j / nbMeridiens;
            float x = rayon * Mathf.Cos(angle);
            float z = rayon * Mathf.Sin(angle);

            vertices[j + 1] = new Vector3(x, 0, z); // Sommet de la base
            uv[j + 1] = new Vector2((float)j / nbMeridiens, 0f); // UV de la base
        }

        // Créer les triangles pour le cône
        for (int j = 0; j < nbMeridiens; j++)
        {
            int a = 0; // Sommet supérieur
            int b = j + 1; // Sommet de la base
            int c = (j + 1) % nbMeridiens + 1; // Sommet suivant de la base

            // Triangle
            triangles[j * 3] = a; // sommet supérieur
            triangles[j * 3 + 1] = c; // sommet suivant de la base
            triangles[j * 3 + 2] = b; // sommet de la base
        }

        // Créer les triangles pour la base
        for (int j = 0; j < nbMeridiens; j++)
        {
            int b = j + 1; // Sommet de la base
            int c = (j + 1) % nbMeridiens + 1; // Sommet suivant de la base
            int centre = nbMeridiens + 1; // Sommet central de la base

            // Triangle pour la base
            triangles[nbMeridiens * 3 + j * 3] = centre; // sommet central
            triangles[nbMeridiens * 3 + j * 3 + 1] = b; // sommet de la base
            triangles[nbMeridiens * 3 + j * 3 + 2] = c; // sommet suivant de la base
        }

        // Affecter les vertices, les UV et les triangles au Mesh
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        // Recalculer les normales pour l'éclairage
        mesh.RecalculateNormals();
    }
}