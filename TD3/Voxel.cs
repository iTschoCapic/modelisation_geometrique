using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Voxel : MonoBehaviour
{
    public class Sphere
    {
        public Vector3 position;
        public float radius;

        public Sphere(Vector3 position, float radius)
        {
            this.position = position;
            this.radius = radius;
        }
    }

    public class BoundingBox
    {
        public Vector3 min;
        public Vector3 max;

        public BoundingBox(Vector3 min, Vector3 max)
        {
            this.min = min;
            this.max = max;
        }
    }

    public static Camera cam;

    public int nOS = 3;
    public List<Sphere> spheres = new List<Sphere>();  // Array storing every sphere in my scene
    public List<BoundingBox> BoundingBoxes = new List<BoundingBox>();

    public int subdivisions = 1;  // Number of subdivisions
    public int resolution = 1; // Number of cube in the initial state
    public GameObject cubePrefab;  // Prefab of the cube
    public GameObject spherePrefab; // Sphere for visual debug
    public GameObject objectPrefab; // Prefab of the object moving around
    public bool union = false;
    public bool intersect = false;

    public List<Color> colors = new List<Color>(); // Adding color for layers makes it easier to debug and see the adaptative construction.

    float step;

    void Start()
    {
        colors.Add(Color.white);
        colors.Add(Color.yellow);
        colors.Add(Color.red);
        colors.Add(Color.green);
        colors.Add(Color.blue);
        colors.Add(Color.black);

        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

        GenerateSpheres(nOS);
        // Add a new sphere that is overlapping the first one (we assume the "player" will have one sphere (nOS(numberOfSphere) = 1)
        spheres.Add(new Sphere(new Vector3(5, 0, 2), 5.0f));
        BoundingBoxes.Add(GenerateBoundingBox(spheres[1]));
        GameObject sphere1 = Instantiate(spherePrefab, new Vector3(5, 0, 2), Quaternion.identity);
        sphere1.GetComponent<Renderer>().material.color = Color.grey;
        nOS++;
        // Need to remove after testing Union and Intersect

        /* I'm adding this GameObject to be fill with a shape.
         * That shape will be on the scene and will interact with
         * my spheres already rendered (Modification are done in
         * the update() function) */
        
        loadPrefab();
        GenerateGlobalRepresentation(nOS);
    }

    public void loadPrefab()
    {
        objectPrefab = Instantiate(objectPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        objectPrefab.GetComponent<Renderer>().material.color = Color.magenta;
    }

    public void addNewSphere(Vector3 vector, float radius)
    {
        spheres.Add(new Sphere(vector, radius));
        BoundingBoxes.Add(GenerateBoundingBox(spheres[spheres.Count-1]));
        GameObject sphere1 = Instantiate(spherePrefab, vector, Quaternion.identity);
        sphere1.GetComponent<Renderer>().material.color = Color.cyan;
        nOS++;
    }

    public void GenerateSpheres(int nOS)
    {
        for (int i = 0; i < nOS; i++)
        {
            spheres.Add(new Sphere(new Vector3(i * 11, 0, 0), 5.0f));
            BoundingBoxes.Add(GenerateBoundingBox(spheres[i]));

            GameObject sphere1 = Instantiate(spherePrefab, new Vector3(i * 11, 0, 0), Quaternion.identity); // Can be disabled, only here for testing purpose
            sphere1.GetComponent<Renderer>().material.color = Color.grey;
        }
    }
    public BoundingBox GenerateBoundingBox(Sphere sphere)
    {
        return new BoundingBox(sphere.position - new Vector3(sphere.radius, sphere.radius, sphere.radius), sphere.position + new Vector3(sphere.radius, sphere.radius, sphere.radius));

    }

    public void GenerateGlobalRepresentation(int nOS)
    {
        for (int i = 0; i < nOS; i++)
        {
            GenerateSphere(spheres[i], BoundingBoxes[i]);
        }
    }

    public void GenerateSphere(Sphere sphere, BoundingBox boundingBox)
    {

        step = (boundingBox.max.x - boundingBox.min.x) / resolution;

        bool[] everyOtherCorner = { false, false, false, false, false, false, false, false };

        bool minCorner = false;
        bool maxCorner = false;

        for (float i = 0; i < resolution; i++)
        {
            for (float j = 0; j < resolution; j++)
            {
                for (float k = 0; k < resolution; k++)
                {
                    float x = boundingBox.min.x + i * step;
                    float y = boundingBox.min.y + j * step;
                    float z = boundingBox.min.z + k * step;

                    Vector3 vector = new Vector3(x, y, z);
                    int numberOfSpheres = 0;

                    minCorner = IntersectSphere(vector, sphere);
                    maxCorner = IntersectSphere(new Vector3(x + step, y + step, z + step), sphere);

                    everyOtherCorner[0] = IntersectSphere(new Vector3(x + step, y, z), sphere);
                    everyOtherCorner[1] = IntersectSphere(new Vector3(x, y + step, z + step), sphere);

                    everyOtherCorner[2] = IntersectSphere(new Vector3(x, y + step, z), sphere);
                    everyOtherCorner[3] = IntersectSphere(new Vector3(x + step, y, z + step), sphere);

                    everyOtherCorner[4] = IntersectSphere(new Vector3(x, y, z + step), sphere);
                    everyOtherCorner[5] = IntersectSphere(new Vector3(x + step, y + step, z), sphere);

                    if (union || intersect)
                    {
                        for (int s = 0; s < nOS; s++)
                        {
                            if (IntersectSphere(new Vector3(x + step / 2, y + step / 2, z + step / 2), spheres[s])) // Checking if the center is intersecting with the bounding box
                            {
                                numberOfSpheres++;
                            }
                        }
                    }

                    if (minCorner && maxCorner && everyOtherCorner[0] && everyOtherCorner[1] && everyOtherCorner[2] && everyOtherCorner[3] && everyOtherCorner[4] && everyOtherCorner[5])
                    {
                        if (union)
                        {
                            // UnionSpheres(numberOfSpheres, new Vector3(vector.x + step / 2, vector.y + step / 2, vector.z + step / 2), step, 0); // Desactivate this one to not have the middle ?
                            // The middle part should be displayed but in my sense it doesn't make much sense and feels more right without it (in certain cases). So I disabled it for now.
                        }
                        else if (intersect)
                        {
                            IntersectSpheres(numberOfSpheres, new Vector3(vector.x + step / 2, vector.y + step / 2, vector.z + step / 2), step, 0);
                        } else
                        {
                            GameObject cube = Instantiate(cubePrefab, new Vector3(vector.x + step / 2, vector.y + step / 2, vector.z + step / 2), Quaternion.identity); // Need to + step / 2 because of the localScale
                            cube.transform.localScale = new Vector3(step, step, step);
                            cube.GetComponent<Renderer>().material.color = Color.cyan;
                        }
                    }
                    else if (minCorner || maxCorner || everyOtherCorner[0] || everyOtherCorner[1] || everyOtherCorner[2] || everyOtherCorner[3] || everyOtherCorner[4] || everyOtherCorner[5])
                    {
                        SphereDivision(vector, step, 0, sphere, 0);
                    }
                }
            }
        }
    }

    public void SphereDivision(Vector3 vector, float step, int currentDivision, Sphere sphere, int numberOfSpheres)
    {
        if (currentDivision >= subdivisions) // Only displaying the last row
        {
            if (union)
            {
                if (null != Physics.OverlapSphere(new Vector3(vector.x + step / 2, vector.y + step / 2, vector.z + step / 2), step)) // Needed to avoid overlapping of cubes but is causing the last "row" to not be displayed because it's overlapping the edge of the rendering sphere (only here for debugging, and easily avoidable by rendering spheres after our cubes) 
                {
                    return;
                }
                UnionSpheres(numberOfSpheres, new Vector3(vector.x + step / 2, vector.y + step / 2, vector.z + step / 2), step, currentDivision);
                return;
            }
            else if (intersect)
            {
                IntersectSpheres(numberOfSpheres, new Vector3(vector.x + step / 2, vector.y + step / 2, vector.z + step / 2), step, currentDivision);
                return;
            }
            else
            {
                GameObject cube = Instantiate(cubePrefab, new Vector3(vector.x + step / 2, vector.y + step / 2, vector.z + step / 2), Quaternion.identity); // Need to + step / 2 because of the localScale
                cube.transform.localScale = new Vector3(step, step, step);
                cube.GetComponent<Renderer>().material.color = colors[currentDivision];
                return;
            }
            
        }

        float newStep = step / 2;

        bool[] everyOtherCorner = { false, false, false, false, false, false };
        bool minCorner = false;
        bool maxCorner = false;

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    float newX = vector.x + i * newStep;
                    float newY = vector.y + j * newStep;
                    float newZ = vector.z + k * newStep;
                    Vector3 newVector = new Vector3(newX, newY, newZ);

                    numberOfSpheres = 0;

                    minCorner = IntersectSphere(newVector, sphere);
                    maxCorner = IntersectSphere(new Vector3(newX + newStep, newY + newStep, newZ + newStep), sphere);

                    everyOtherCorner[0] = IntersectSphere(new Vector3(newX + newStep, newY, newZ), sphere);
                    everyOtherCorner[1] = IntersectSphere(new Vector3(newX, newY + newStep, newZ + newStep), sphere);

                    everyOtherCorner[2] = IntersectSphere(new Vector3(newX, newY, newZ + newStep), sphere);
                    everyOtherCorner[3] = IntersectSphere(new Vector3(newX + newStep, newY + newStep, newZ), sphere);

                    everyOtherCorner[4] = IntersectSphere(new Vector3(newX, newY + newStep, newZ), sphere);
                    everyOtherCorner[5] = IntersectSphere(new Vector3(newX + newStep, newY, newZ + newStep), sphere);

                    if (union || intersect)
                    {
                        for (int s = 0; s < nOS; s++)
                        {
                            if (IntersectSphere(new Vector3(newX + newStep / 2, newY + newStep / 2, newZ + newStep / 2), spheres[s])) // Checking if the center is intersecting with the bounding box
                            {
                                numberOfSpheres++;
                            }
                        }
                    }

                    if (minCorner && maxCorner && everyOtherCorner[0] && everyOtherCorner[1] && everyOtherCorner[2] && everyOtherCorner[3] && everyOtherCorner[4] && everyOtherCorner[5])
                    {
                        if (union)
                        {
                            UnionSpheres(numberOfSpheres, new Vector3(newX + newStep / 2, newY + newStep / 2, newZ + newStep / 2), newStep, currentDivision);
                        }
                        else if (intersect)
                        {
                            if (currentDivision < subdivisions) // Needed to smoothen the edge (which can be the center of the sphere in which case is a big cube) to smaller ones.
                            {
                                if (numberOfSpheres > 1) // Ensure that the intersection is "right" (more than 1 sphere is on this cube)
                                {
                                    SphereDivision(newVector, newStep, currentDivision+1, sphere, numberOfSpheres);
                                }
                            }
                        }
                        else
                        {
                            GameObject cube = Instantiate(cubePrefab, new Vector3(newX + newStep / 2, newY + newStep / 2, newZ + newStep / 2), Quaternion.identity);
                            cube.transform.localScale = new Vector3(newStep, newStep, newStep);
                            cube.GetComponent<Renderer>().material.color = colors[currentDivision];
                        }
                        
                    }
                    else if (minCorner || maxCorner || everyOtherCorner[0] || everyOtherCorner[1] || everyOtherCorner[2] || everyOtherCorner[3] || everyOtherCorner[4] || everyOtherCorner[5])
                    {
                        SphereDivision(newVector, newStep, currentDivision + 1, sphere, numberOfSpheres);
                    }
                }
            }
        }
    }

    public bool IntersectSphere(Vector3 vec, Sphere sphere)
    {
        if (Vector3.Distance(vec, sphere.position) < sphere.radius)
        {
            return true;
        }
        return false;
    }

    public void UnionSpheres(int numberOfSpheres, Vector3 vector, float step, int currentDivision)
    {
        if (numberOfSpheres == 1)
        {
            GameObject cube = Instantiate(cubePrefab, vector, Quaternion.identity);
            cube.transform.localScale = new Vector3(step, step, step);
            cube.GetComponent<Renderer>().material.color = colors[currentDivision];
        }
    }

    public void IntersectSpheres(int numberOfSpheres, Vector3 vector, float step, int currentDivision)
    {
        if (numberOfSpheres > 1)
        {
            GameObject cube = Instantiate(cubePrefab, vector, Quaternion.identity);
            cube.transform.localScale = new Vector3(step, step, step);
            cube.GetComponent<Renderer>().material.color = colors[currentDivision];
        }
    }

    public bool CameraToMouseRay(out RaycastHit hit)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit);
    }

    public void cleanScene()
    {
        foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
        {
            if (o.name.Contains("GameObject") || o.name.Contains("Main Camera") || o.name.Contains("Directional Light") || o.name.Contains("ObjectPrefab"))
            {
                continue;
            }
            Destroy(o);
        }
        GenerateGlobalRepresentation(nOS);
    }
}