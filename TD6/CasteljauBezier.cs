using UnityEngine;
using System.Collections.Generic;

public class CasteljauBezier : MonoBehaviour
{
    public List<Transform> controlPoints; // Liste des points de contr�le
    public int resolution = 100; // R�solution de la courbe
    public float t = 0.5f; // Param�tre t pour obtenir un point sur la courbe

    private void OnDrawGizmos()
    {
        // Affichage des points de contr�le
        Gizmos.color = Color.red;
        foreach (var point in controlPoints)
        {
            Gizmos.DrawSphere(point.position, 0.1f);
        }

        // Tracer la courbe de B�zier avec l'algorithme de Casteljau
        Vector3 previousPoint = controlPoints[0].position;
        for (int i = 1; i <= resolution; i++)
        {
            float time = i / (float)resolution;
            Vector3 point = CalculateCasteljauPoint(time);
            Gizmos.DrawLine(previousPoint, point); // Dessiner la ligne entre les points successifs
            previousPoint = point;
        }

        // Dessiner le polygone reliant les points de contr�le
        Gizmos.color = Color.blue;
        for (int i = 0; i < controlPoints.Count - 1; i++)
        {
            Gizmos.DrawLine(controlPoints[i].position, controlPoints[i + 1].position);
        }
    }

    // Calculer un point de la courbe de B�zier avec l'algorithme de Casteljau
    Vector3 CalculateCasteljauPoint(float t)
    {
        List<Vector3> tempPoints = new List<Vector3>();

        // Copier les points de contr�le dans une liste temporaire
        foreach (var point in controlPoints)
        {
            tempPoints.Add(point.position);
        }

        // Appliquer l'algorithme de Casteljau
        int n = tempPoints.Count;
        for (int r = 1; r < n; r++) // Pour chaque niveau
        {
            for (int i = 0; i < n - r; i++) // Pour chaque point dans le niveau
            {
                tempPoints[i] = Vector3.Lerp(tempPoints[i], tempPoints[i + 1], t);
            }
        }

        // Le dernier point restant est le point sur la courbe de B�zier
        return tempPoints[0];
    }

    // Permettre � l'utilisateur de modifier le param�tre t avec les touches
    private void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
            t = Mathf.Min(1f, t + 0.01f); // Augmenter t
        if (Input.GetKey(KeyCode.DownArrow))
            t = Mathf.Max(0f, t - 0.01f); // Diminuer t
    }
}
