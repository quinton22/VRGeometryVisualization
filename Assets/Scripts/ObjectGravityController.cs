using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGravityController : MonoBehaviour
{
    private bool _isGravityOn = false;
    public bool isGravityOn { get { return _isGravityOn; } }

    public void ToggleGravity()
    {
        _isGravityOn = !_isGravityOn;
        UpdateGravity();
    }

    void SetGravity(bool on)
    {
        _isGravityOn = on;
        UpdateGravity();
    }

    void UpdateGravity()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            Transform currentObject = transform.GetChild(i);
            if (isGravityOn) GravityOn(currentObject);
            else GravityOff(currentObject);
        }
    }

    void GravityOn(Transform t)
    {
        Rigidbody rigidbody = t.GetComponent<Rigidbody>();
        if (rigidbody == null) rigidbody = t.gameObject.AddComponent<Rigidbody>();

        rigidbody.isKinematic = false;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    void GravityOff(Transform t)
    {
        Rigidbody rigidbody = t.GetComponent<Rigidbody>();
        if (rigidbody == null) return;

        Destroy(rigidbody);
    }

}
