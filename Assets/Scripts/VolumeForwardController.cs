using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// VolumeForwardController
// Determines the forward direction of a volume and allows for access by other scripts
public class VolumeForwardController : MonoBehaviour
{
    public Vector3 ZDirection;
    public Vector3 dir;

    public float result;
    public List<string> rotated = new List<string>();

    public float Result()
    {
        if (ZDirection != null && dir != null && ZDirection != Vector3.zero && dir != Vector3.zero)
        {
            result = Vector3.Magnitude(ZDirection.normalized - dir.normalized);
            return result;
        }
        return 1;
    }
}
