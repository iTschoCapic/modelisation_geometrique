using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PacmanGenerator : MonoBehaviour
{
    public float rayon = 1.0f;           // Rayon de la sphère (corps de Pac-Man)
    public int nbParalleles = 10;        // Nombre de parallèles
    public int nbMeridiens = 20;         // Nombre de méridiens
    public float angleOuverture = 45f;   // Angle d'ouverture de la "bouche" en degrés

    void Start()
    {
        // Vérifier et ajouter les composants nécessaires (MeshFilter et MeshRenderer)
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        // Créer un nouveau Mesh
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        // Convertir l'angle d'ouverture en radians
        float ouvertureRadian = angleOuverture * Mathf.Deg2Rad;

        // Calculer le nombre de sommets (sphère tronquée + centres des disques)
        int nbSommets = (nbParalleles + 1) * (nbMeridiens + 1) + 2; // Sommets de la sphère + centres des disques (haut/bas)
        Vector3[] vertices = new Vector3[nbSommets];
        Vector2[] uv = new Vector2[nbSommets];
        int[] triangles = new int[nbParalleles * nbMeridiens * 6 + nbMeridiens * 6]; // Triangles pour la sphère et la bouche

        // Créer les sommets de la sphère tronquée (Pac-Man)
        int vertIndex = 0;
        for (int i = 0; i <= nbParalleles; i++)
        {
            float latitude = Mathf.PI * (-0.5f + (float)i / nbParalleles); // De -90 à 90 degrés
            for (int j = 0; j <= nbMeridiens; j++)
            {
                float longitude = (Mathf.PI - ouvertureRadian) + 2 * (Mathf.PI - ouvertureRadian) * (float)j / nbMeridiens;

                float x = rayon * Mathf.Cos(latitude) * Mathf.Cos(longitude);
                float y = rayon * Mathf.Sin(latitude);
                float z = rayon * Mathf.Cos(latitude) * Mathf.Sin(longitude);

                vertices[vertIndex] = new Vector3(x, y, z);
                uv[vertIndex] = new Vector2((float)j / nbMeridiens, (float)i / nbParalleles);
                vertIndex++;
            }
        }

        // Créer les triangles de la sphère
        int triIndex = 0;
        for (int i = 0; i < nbParalleles; i++)
        {
            for (int j = 0; j < nbMeridiens; j++)
            {
                int a = i * (nbMeridiens + 1) + j;     // Sommet en bas à gauche
                int b = a + nbMeridiens + 1;           // Sommet en haut à gauche
                int c = a + 1;                         // Sommet en bas à droite
                int d = b + 1;                         // Sommet en haut à droite

                // Premier triangle
                triangles[triIndex++] = a;
                triangles[triIndex++] = b;
                triangles[triIndex++] = c;

                // Deuxième triangle
                triangles[triIndex++] = b;
                triangles[triIndex++] = d;
                triangles[triIndex++] = c;
            }
        }

        ////Ne marche pas encore mais je sais pas pourquoi :(
        //// Ajouter les sommets centraux pour les deux disques (bouche)
        //int centreInferieurIndex = vertIndex; // Centre du disque inférieur
        //vertices[vertIndex++] = new Vector3(0, -rayon, 0); // Centre bas

        //int centreSuperieurIndex = vertIndex; // Centre du disque supérieur
        //vertices[vertIndex++] = new Vector3(0, rayon, 0); // Centre haut

        //// Créer les triangles pour fermer la bouche en bas
        //for (int j = 0; j < nbMeridiens; j++)
        //{
        //    int baseA = j;                    // Point du premier parallèle
        //    int baseB = j + 1;                // Point suivant sur le même parallèle

        //    triangles[triIndex++] = centreInferieurIndex;
        //    triangles[triIndex++] = baseB;
        //    triangles[triIndex++] = baseA;
        //}

        //// Créer les triangles pour fermer la bouche en haut
        //for (int j = 0; j < nbMeridiens; j++)
        //{
        //    int baseA = (nbParalleles) * (nbMeridiens + 1) + j; // Dernière ligne de la sphère
        //    int baseB = baseA + 1;

        //    triangles[triIndex++] = centreSuperieurIndex;
        //    triangles[triIndex++] = baseA;
        //    triangles[triIndex++] = baseB;
        //}

        // Affecter les vertices, les UV et les triangles au Mesh
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        // Recalculer les normales pour l'éclairage
        mesh.RecalculateNormals();
    }
}
