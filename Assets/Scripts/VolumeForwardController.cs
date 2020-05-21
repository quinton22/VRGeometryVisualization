using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// VolumeForwardController
// Determines the forward direction of a volume and allows for access by other scripts
public class VolumeForwardController : MonoBehaviour
{
    [System.NonSerialized]
    public Vector3 ZDirection;
    [System.NonSerialized]
    public Vector3 dir;
    [System.NonSerialized]
    public float result;
    [System.NonSerialized]
    public List<string> rotated = new List<string>(); // TODO: remove?

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
