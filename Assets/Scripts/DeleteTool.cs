using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class DeleteTool : MonoBehaviour
{
    public SteamVR_Action_Boolean m_DeleteAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Draw");
    private Interactable interactable;
    private GameObject _intersectingObject;
    public GameObject intersectingObject {
        get { return _intersectingObject; }
        set {
            _intersectingObject = FindClosestDeletableObject(value);
        }
    }
    public bool isIntersecting { get { return intersectingObject != null; } }

    void Start()
    {
        interactable = GetComponent<Interactable>();
        m_DeleteAction.AddOnStateDownListener(ButtonDown, SteamVR_Input_Sources.LeftHand);
        m_DeleteAction.AddOnStateDownListener(ButtonDown, SteamVR_Input_Sources.RightHand);
    }

    void Update()
    {
        
    }

    public void ButtonDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (interactable.attachedToHand == null) return;

        if (interactable.attachedToHand.handType != fromSource) return; // if not the same hand, don't do anything

        Delete();
    }

    public void Delete()
    {
        if (isIntersecting)
        {
            Destroy(intersectingObject);
        }
    }

    static GameObject FindClosestDeletableObject(GameObject obj)
    {
        if (obj == null) return null;
        
        Transform t = obj.transform;

        do
        {
            // find ancestor with name containing Line, Area, Volume, Mesh
            if (t.name.ToLower().Contains("line") ||
                t.name.ToLower().Contains("area") ||
                t.name.ToLower().Contains("volume") ||
                t.name.ToLower().Contains("mesh"))
                return t.gameObject;

            t = t.parent;
        }
        while (t.name != t.root.name);

        return null;
    }
}
