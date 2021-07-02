using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltController : MonoBehaviour
{
    BeltColliderController[] beltColliders;
    int lastIndex = -1;
    // Start is called before the first frame update
    void Start()
    {
        beltColliders = gameObject.GetComponentsInChildren<BeltColliderController>();
    }

    void Update()
    {
        for (int i = 0; i < beltColliders.Length; ++i)
        {
            if (lastIndex != i && beltColliders[i].isObjectAttached) {
                lastIndex = i;
                break;
            }
        }
    }

    public void goToLastCollider(GameObject obj)
    {
        if (lastIndex == -1) return;

        beltColliders[lastIndex].attachObject(obj);
    }
}
