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
    private Vector3 PointerPlaneXAxis;
    private Vector3 PointerPlaneYAxis;
    private Vector3 PointerPlaneOrigin;
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

        PointerPlaneOrigin = a;

        // b - a gives the grid's y axis
        PointerPlaneYAxis = Vector3.Normalize(b-PointerPlaneOrigin);

        // vector that is perp to the y axis and the z axis, which is perp to y axis and the vector c-a
        PointerPlaneXAxis = -Vector3.Cross(PointerPlaneYAxis, Vector3.Cross(PointerPlaneYAxis, c-PointerPlaneOrigin)).normalized;
    }

    // return the closest point to <initialPoint> on the plane which is on a grid which size is stated according to <scale>
    public Vector3 GetGridPointFromPlane(Vector3 initialPoint, float scale)
    {
        if (!isCentered)
        {
            Vector3 closestPoint = PointerPlane.ClosestPointOnPlane(initialPoint);

            // want to take closestPoint and round it to the nearest point such that
            // the point = a * PointerPlaneXAxis + b * PointerPlaneYAxis and
            // a * scale & b * scale are integers
            
            Vector3 closestPointX = Vector3.Project(closestPoint - PointerPlaneOrigin, PointerPlaneXAxis);
            Vector3 closestPointY = Vector3.Project(closestPoint - PointerPlaneOrigin, PointerPlaneYAxis);

            float targetXMagnitude = Mathf.Round(closestPointX.magnitude * scale) / scale;
            float targetYMagnitude = Mathf.Round(closestPointY.magnitude * scale) / scale;

            // normal of the (signed) axes * target magnitudes = new point on grid
            return PointerPlaneOrigin + closestPointX.normalized * targetXMagnitude + closestPointY.normalized * targetYMagnitude;
        }
        
        throw new UnassignedReferenceException("PointerPlane is not set.");
    }
}
