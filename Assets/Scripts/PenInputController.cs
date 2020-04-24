using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class PenInputController : MonoBehaviour
{
    public InputController m_InputController;
    public SteamVR_Action_Boolean m_DrawAction;
    private Interactable interactable;
    private bool isTriggerDown = false;
    private FixedJoint fixedJoint;
    private Vector3 anchor;
    private bool isJointAttached = false;

    void Start()
    {
        interactable = GetComponent<Interactable>();
        fixedJoint = GetComponent<FixedJoint>();
        anchor = transform.Find("AttachPoint").localPosition;
    }

    void Update()
    {
        if (interactable.attachedToHand != null)
        {
            SteamVR_Input_Sources source = interactable.attachedToHand.handType;
            
            if (m_DrawAction[source].stateDown) // trigger down
            {
                if (!isTriggerDown)
                    StartDraw();
                else
                    Drawing();
            }
            else if (m_DrawAction[source].stateUp) // trigger up
            {
                StopDraw();
            }
       
        }

        if (isJointAttached && fixedJoint.connectedBody == null)
        {
            isJointAttached = false;
            m_InputController.SetTool(InputController.Tool.None);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (fixedJoint.connectedBody == null) return;

        string name = other.gameObject.name.ToLower();
        if (name.Contains("line") || name.Contains("area") || name.Contains("cube") || name.Contains("mesh"))
        {
            fixedJoint.connectedBody = other.attachedRigidbody;
            fixedJoint.anchor = anchor;
            isJointAttached = true;

            if (name.Contains("line"))
                m_InputController.SetTool(InputController.Tool.Line);
            else if (name.Contains("area"))
                m_InputController.SetTool(InputController.Tool.Area);
            else if (name.Contains("cube"))
                m_InputController.SetTool(InputController.Tool.Volume);
            else if (name.Contains("mesh"))
                m_InputController.SetTool(InputController.Tool.Mesh);
        }
    }

    void StartDraw()
    {
        isTriggerDown = true;
        m_InputController.Clicked();
    }

    void Drawing()
    {
        m_InputController.Dragged();
    }

    void StopDraw()
    {
        isTriggerDown = false;
        m_InputController.MouseUp();
    }
}
