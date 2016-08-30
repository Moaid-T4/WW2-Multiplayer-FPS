using UnityEngine;
using System.Collections;

public class Globe : MonoBehaviour {

    [SerializeField]
    Transform globe;

    [SerializeField]
    internal bool canRotate;

    [SerializeField]
    Vector2 rotationDelta;

    [SerializeField]
    Vector3 rotation;

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
            canRotate = false;

        if (canRotate)
        {
            transform.Rotate(0, -Input.GetAxis("Mouse X") * rotationDelta.y, 0, Space.World);

            globe.Rotate(Input.GetAxis("Mouse Y") * rotationDelta.x, 0, 0, Space.World);
        }
    }

    void OnMouseDown()
    {
        canRotate = true;
    }
}
