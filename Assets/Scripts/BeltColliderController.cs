using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class BeltColliderController : MonoBehaviour
{
    public bool isObjectAttached {
        get { return attachedObject != null;}
    }
    private GameObject attachedObject;
    private Quaternion lastRotation;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (attachedObject != null)
        {
            // TODO: Do we want to move this to FixedUpdate?
            if (isAttachedObjectGrabbed())
            {
                // unattach
                attachedObject = null;
            }
            else
            {
                // update position of attached object to equal that of this gameobject
                attachedObject.transform.position = transform.position;
                attachedObject.transform.rotation = lastRotation;
                
                // TODO: what rotation do we want on the object?
                // leave rotation for now
            }

           

        }
    }

    void OnTriggerEnter(Collider other)
    {
        Player player = Player.instance;
        if (other.gameObject.name == "drawingTool" )
        {
           if ((player.rightHand.currentAttachedObject == null ||  player.rightHand.currentAttachedObject.name != "drawingTool")
                && (player.leftHand.currentAttachedObject == null || player.leftHand.currentAttachedObject.name != "drawingTool"))
           {
               // attach object
               attachObject(other.gameObject);
           }
        }
    }

    private bool isAttachedObjectGrabbed()
    {
        Player player = Player.instance;
        bool isGrabbedInRightHand = player.rightHand.currentAttachedObject != null && player.rightHand.currentAttachedObject.name == attachedObject.name;
        bool isGrabbedInLeftHand = player.leftHand.currentAttachedObject != null && player.leftHand.currentAttachedObject.name == attachedObject.name;
        return isGrabbedInLeftHand || isGrabbedInRightHand;
    }

    public void attachObject(GameObject obj)
    {
        attachedObject = obj;
        Rigidbody rb = attachedObject.GetComponent<Rigidbody>();
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
        lastRotation = obj.transform.rotation;

        // disable rigidbody collisions?
    }
}
