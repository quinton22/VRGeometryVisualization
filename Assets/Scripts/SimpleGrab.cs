using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
public class SimpleGrab : MonoBehaviour
{
    public bool ShowHint = false;
    private Interactable interactable;
    [EnumFlags]
    public Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.TurnOnKinematic;

    void Start()
    {
        interactable = gameObject.GetComponent<Interactable>();
    }

    private void OnHandHoverBegin(Hand hand)
    {
        if (ShowHint)
            hand.ShowGrabHint();
    }

    private void OnHandHoverEnd(Hand hand)
    {
        if (ShowHint)
            hand.HideGrabHint();
    }

    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes grabType = hand.GetGrabStarting();
        bool isGrabEnding = hand.IsGrabEnding(gameObject);

        if (interactable.attachedToHand == null && grabType != GrabTypes.None)
        {
            hand.AttachObject(gameObject, grabType, attachmentFlags);
            hand.HoverLock(interactable);
            hand.HideGrabHint();
        }
        else if (isGrabEnding)
        {
            hand.DetachObject(gameObject);
            hand.HoverUnlock(interactable);
        }
    }
}
