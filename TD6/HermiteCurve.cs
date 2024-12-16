using UnityEngine;

public class HermiteCurve : MonoBehaviour
{
    public Transform P0; // Premier point de la courbe
    public Transform P1; // Deuxième point de la courbe
    public Transform V0; // Tangente au point P0
    public Transform V1; // Tangente au point P1

    [Range(0, 1)] public float t = 0; // Paramètre t pour contrôler la position sur la courbe
    public int resolution = 20; // Résolution de la courbe (nombre de points à calculer)

    private void OnDrawGizmos()
    {
        // Tracer la courbe d'Hermite
        Vector3 previousPoint = P0.position;

        // Calculer les points de la courbe en fonction du paramètre t
        for (int i = 1; i <= resolution; i++)
        {
            float time = i / (float)resolution;
            Vector3 point = CalculateHermitePoint(time);
            Gizmos.DrawLine(previousPoint, point); // Dessiner la ligne entre les points successifs
            previousPoint = point;
        }
    }

    // Méthode pour calculer un point sur la courbe d'Hermite pour un paramètre t donné
    Vector3 CalculateHermitePoint(float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        // Calcul des coefficients de la courbe d'Hermite
        float h0 = 2 * t3 - 3 * t2 + 1;
        float h1 = -2 * t3 + 3 * t2;
        float h2 = t3 - 2 * t2 + t;
        float h3 = t3 - t2;

        // Calcul du point final
        Vector3 point = h0 * P0.position + h1 * V0.position + h2 * P1.position + h3 * V1.position;

        return point;
    }
}
