using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SphereScaleController
// controls the scale of the sphere attached to the line of a mesh
public class SphereScaleController : MonoBehaviour
{
    private float scale = .1f;

    void Update()
    {
        Vector3 s = transform.localScale;
        if (transform.parent.localScale.y != 0)
        {
            s.y = scale / transform.parent.localScale.y;
        }
        else
        {
            s.y = 0;
        }
        transform.localScale = s;

    }
}
