using UnityEngine;

public class SphereGenerator : MonoBehaviour
{
    public float rayon = 1.0f;           // Rayon de la sph�re
    public int nbParalleles = 10;        // Nombre de parall�les
    public int nbMeridiens = 20;         // Nombre de m�ridiens

    void Start()
    {
        // Cr�er un nouveau Mesh
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Calculer le nombre de sommets
        int nbSommets = (nbParalleles + 1) * (nbMeridiens + 1);
        Vector3[] vertices = new Vector3[nbSommets];
        Vector2[] uv = new Vector2[nbSommets];
        int[] triangles = new int[nbParalleles * nbMeridiens * 6]; // 2 triangles par rectangle

        // Cr�er les sommets de la sph�re
        int vertIndex = 0;
        for (int i = 0; i <= nbParalleles; i++)
        {
            float latitude = Mathf.PI * (-0.5f + (float)i / nbParalleles); // De -90 � 90 degr�s
            for (int j = 0; j <= nbMeridiens; j++)
            {
                float longitude = 2 * Mathf.PI * (float)j / nbMeridiens; // De 0 � 360 degr�s
                float x = rayon * Mathf.Cos(latitude) * Mathf.Cos(longitude);
                float y = rayon * Mathf.Sin(latitude);
                float z = rayon * Mathf.Cos(latitude) * Mathf.Sin(longitude);

                vertices[vertIndex] = new Vector3(x, y, z);
                uv[vertIndex] = new Vector2((float)j / nbMeridiens, (float)i / nbParalleles);
                vertIndex++;
            }
        }

        // Cr�er les triangles
        int triIndex = 0;
        for (int i = 0; i < nbParalleles; i++)
        {
            for (int j = 0; j < nbMeridiens; j++)
            {
                int a = i * (nbMeridiens + 1) + j;     // Sommet en bas � gauche
                int b = a + nbMeridiens + 1;           // Sommet en haut � gauche
                int c = a + 1;                          // Sommet en bas � droite
                int d = b + 1;                          // Sommet en haut � droite

                // Premier triangle
                triangles[triIndex++] = a;
                triangles[triIndex++] = b;
                triangles[triIndex++] = c;

                // Deuxi�me triangle
                triangles[triIndex++] = b;
                triangles[triIndex++] = d;
                triangles[triIndex++] = c;
            }
        }

        // Affecter les vertices, les UV et les triangles au Mesh
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        // Recalculer les normales pour l'�clairage
        mesh.RecalculateNormals();
    }
}