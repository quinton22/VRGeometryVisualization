using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PointerController
// Controls the location of the center pointer
public class PointerController : MonoBehaviour
{

    public GameObject collidingObject;
    public bool isCentered = true;
    public bool wasCentered = true;
    private Plane PointerPlane;
    private Vector3 m_InitialLocalPosition;
    // Start is called before the first frame update
    void Start()
    {
        m_InitialLocalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCentered)
        {
            transform.position = PointerPlane.ClosestPointOnPlane(transform.parent.TransformPoint(m_InitialLocalPosition));

            if (wasCentered)
                wasCentered = false;
        }
        else if (!wasCentered)
        {
            transform.localPosition = m_InitialLocalPosition;
            wasCentered = true;
        }
    }

    public void SetPointerPlane(Vector3 a, Vector3 b, Vector3 c)
    {
        PointerPlane = new Plane(a, b, c);
    }
}
