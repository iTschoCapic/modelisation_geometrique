using UnityEngine;

public class Move : MonoBehaviour
{
    private bool selected;
    private Vector3 startingPos;

    private Voxel v;

    void Start()
    {
        v = GameObject.Find("GameObject").GetComponent<Voxel>();
        selected = true;
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (selected)
        {
            RaycastHit hit;
            if (v.CameraToMouseRay(out hit))
            {
                transform.position = hit.point + Vector3.up * transform.localScale.y;
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            setSelected();
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            setSelected();
            v.intersect = !v.intersect;
            v.addNewSphere(transform.position, transform.localScale.x / 2);
            v.cleanScene();
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            setSelected();
            v.union = !v.union;
            v.addNewSphere(transform.position, transform.localScale.x / 2);
            v.cleanScene();
        }
    }

    void setSelected()
    {
        this.selected = !selected;
    }
}
