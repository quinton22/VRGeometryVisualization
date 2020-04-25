using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PointerController
// Controls the location of the center pointer
public class PointerController : MonoBehaviour
{
    [System.NonSerialized]
    public GameObject collidingObject;
    [System.NonSerialized]
    public bool isCentered = true;
    [System.NonSerialized]
    public bool wasCentered = true;
    private Plane PointerPlane;
    private Vector3 PointerPlaneXAxis;
    private Vector3 PointerPlaneYAxis;
    private Vector3 PointerPlaneOrigin;
    private Vector3 m_InitialLocalPosition;
    private bool RestrictToInsideRays = false;
    private Vector3 RayOrigin;
    private Vector3 Dir1;
    private Vector3 Dir2;

    [Header("Scale Pointer")]
    [SerializeField]
    [Tooltip("Time, in seconds, it takes pointer to scale up when drawing")]
    private float m_ScaleTime = .2f;
    [SerializeField]
    [Tooltip("Maximum times the initial scale that the pointer increases")]
    private float m_MaxScale = 5;
    private Vector3 initialScale;
    private int scaleDirection = 0; // -1 for decreasing, 0 for no change, 1 for incr
    private float currentScaleTime = 0;
    [SerializeField]
    [Tooltip("Scale of pointer increases the farther you are away")]
    private bool m_ScaleIncreaseWithDistance = false;
    [SerializeField]
    [Tooltip("Maximum multiplier on scale")]
    private float m_MaxDistanceScale = 2; 
    
    // Start is called before the first frame update
    void Start()
    {
        m_InitialLocalPosition = transform.localPosition;
        initialScale = transform.localScale;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!isCentered)
        {
            Vector3 centerPoint = transform.parent.TransformPoint(m_InitialLocalPosition);

            centerPoint = PointerPlane.ClosestPointOnPlane(centerPoint);
            
            transform.position = centerPoint;

            if (RestrictToInsideRays)
            {
                Vector3 dir3 = (centerPoint - RayOrigin).normalized;

                if (!(Vector3.Dot(Vector3.Cross(Dir1, dir3), Vector3.Cross(Dir1, Dir2)) >= 0
                    && Vector3.Dot(Vector3.Cross(Dir2, dir3), Vector3.Cross(Dir2, Dir1)) >= 0))
                {
                    // C is not inside A and B
                    Vector3 potentialPoint1 = RayOrigin + Dir1 * Vector3.Dot(Dir1, centerPoint - RayOrigin);
                    Vector3 potentialPoint2 = RayOrigin + Dir2 * Vector3.Dot(Dir2, centerPoint - RayOrigin);

                    bool A = ((potentialPoint1 - RayOrigin).normalized - Dir1).magnitude < 0.0001f;
                    bool B = ((potentialPoint2 - RayOrigin).normalized - Dir2).magnitude < 0.0001f;

                    if (A && B)
                    {
                        transform.position = (potentialPoint1 - centerPoint).magnitude < (potentialPoint2 - centerPoint).magnitude
                            ? potentialPoint1
                            : potentialPoint2;
                    }
                    else if (A)
                    {
                        transform.position = potentialPoint1;
                    }
                    else if (B)
                    {
                        transform.position = potentialPoint2;
                    }
                    else
                    {
                        transform.position = RayOrigin;
                    }                    
                }
            }

            if (wasCentered)
                wasCentered = false;
        }
        else if (!wasCentered)
        {
            transform.localPosition = m_InitialLocalPosition;
            wasCentered = true;
        }

        if (scaleDirection != 0)
        {
            currentScaleTime += scaleDirection * Time.deltaTime;
            if (currentScaleTime > m_ScaleTime) 
            {
                currentScaleTime = m_ScaleTime;
                scaleDirection = 0;
            }
            else if (currentScaleTime < 0)
            {
                currentScaleTime = 0;
                scaleDirection = 0;
            }

            
            transform.localScale = Vector3.Lerp(initialScale, initialScale * m_MaxScale, currentScaleTime / m_ScaleTime);
        }
        else if (m_ScaleIncreaseWithDistance && !isCentered)
            transform.localScale = Vector3.Lerp(initialScale * m_MaxScale, initialScale * m_MaxScale * m_MaxDistanceScale, transform.localPosition.magnitude / 100);
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

    public void RestrictPointerToInsideRays(Vector3 origin, Vector3 dir1, Vector3 dir2)
    {
        RestrictToInsideRays = true;
        RayOrigin = origin;
        Dir1 = dir1.normalized;
        Dir2 = dir2.normalized;
    }

    public void UnrestrictPointerToInsideRays()
    {
        RestrictToInsideRays = false;
    }

    public void IncreaseSizeOfPointer()
    {
        scaleDirection = 1;
    }

    public void DecreaseSizeOfPointer()
    {
        scaleDirection = -1;
    }
}
