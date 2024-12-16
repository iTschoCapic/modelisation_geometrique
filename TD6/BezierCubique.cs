using UnityEngine;

public class BezierCubique : MonoBehaviour
{
    public Transform P0, P1, P2, P3; // Les 4 points de contr�le
    public int resolution = 100; // R�solution de la courbe
    private Transform selectedPoint; // Le point s�lectionn�

    private void OnDrawGizmos()
    {
        // Afficher le polygone reliant les points de contr�le
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(P0.position, P1.position);
        Gizmos.DrawLine(P1.position, P2.position);
        Gizmos.DrawLine(P2.position, P3.position);

        // Calculer et dessiner la courbe de B�zier
        Vector3 previousPoint = P0.position;
        for (int i = 1; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            Vector3 point = CalculateBezierPoint(t);
            Gizmos.DrawLine(previousPoint, point);
            previousPoint = point;
        }

        // Dessiner les points de contr�le (petits cubes)
        Gizmos.color = Color.red;
        Gizmos.DrawCube(P0.position, Vector3.one * 0.2f);
        Gizmos.DrawCube(P1.position, Vector3.one * 0.2f);
        Gizmos.DrawCube(P2.position, Vector3.one * 0.2f);
        Gizmos.DrawCube(P3.position, Vector3.one * 0.2f);

        // Mettre en surbrillance le point s�lectionn�
        if (selectedPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(selectedPoint.position, Vector3.one * 0.3f);
        }
    }

    // Calculer un point de la courbe de B�zier pour un param�tre t
    Vector3 CalculateBezierPoint(float t)
    {
        float u = 1 - t;
        float t2 = t * t;
        float t3 = t2 * t;
        float u2 = u * u;
        float u3 = u2 * u;

        Vector3 point = u3 * P0.position + 3 * u2 * t * P1.position + 3 * u * t2 * P2.position + t3 * P3.position;
        return point;
    }

    private void Update()
    {
        // S�lectionner le point de contr�le avec les touches 0, 1, 2, 3
        if (Input.GetKeyDown(KeyCode.Alpha0)) selectedPoint = P0;
        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedPoint = P1;
        if (Input.GetKeyDown(KeyCode.Alpha2)) selectedPoint = P2;
        if (Input.GetKeyDown(KeyCode.Alpha3)) selectedPoint = P3;

        // D�placer le point s�lectionn� avec les touches z, d, q, s
        if (selectedPoint != null)
        {
            if (Input.GetKey(KeyCode.Z)) selectedPoint.position += Vector3.up * 0.1f;
            if (Input.GetKey(KeyCode.D)) selectedPoint.position += Vector3.right * 0.1f;
            if (Input.GetKey(KeyCode.Q)) selectedPoint.position += Vector3.left * 0.1f;
            if (Input.GetKey(KeyCode.S)) selectedPoint.position += Vector3.down * 0.1f;
        }
    }
}
