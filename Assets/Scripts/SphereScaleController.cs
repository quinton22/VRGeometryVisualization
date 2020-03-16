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
        if (transform.parent.localScale.y != 0)
        {
            Vector3 s = transform.localScale;
            s.y = scale / transform.parent.localScale.y;
            transform.localScale = s;
        }
    }
}
