using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class PenInputController : MonoBehaviour
{
    public InputController m_InputController;
    public SteamVR_Action_Boolean m_DrawAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Draw");
    public SteamVR_Action_Boolean m_SwitchToolAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("SwitchTool");
    private Interactable interactable;
    private bool isTriggerDown = false;
    private Transform attachments;
    private int currentAttachment = -1;
    private bool shownDrawHint = false;
    private bool shownChangeToolHint = false;
    private Hand hintHand;
    void Start()
    {
        interactable = GetComponent<Interactable>();

        m_DrawAction.AddOnStateDownListener(TriggerDown, SteamVR_Input_Sources.LeftHand);
        m_DrawAction.AddOnStateDownListener(TriggerDown, SteamVR_Input_Sources.RightHand);
        m_DrawAction.AddOnStateUpListener(TriggerUp, SteamVR_Input_Sources.LeftHand);
        m_DrawAction.AddOnStateUpListener(TriggerUp, SteamVR_Input_Sources.RightHand);

        m_SwitchToolAction.AddOnStateDownListener(ButtonDown, SteamVR_Input_Sources.LeftHand);
        m_SwitchToolAction.AddOnStateDownListener(ButtonDown, SteamVR_Input_Sources.RightHand);

        attachments = transform.Find("Attachments");
    }

    void Update()
    {
        if (isTriggerDown)
        {
            Drawing();
        }

        if (interactable.attachedToHand != null)
        {
            if (!shownChangeToolHint)
            {
                // TODO: add button to toggle hints
                ControllerButtonHints.ShowButtonHint(interactable.attachedToHand, m_SwitchToolAction);
                ControllerButtonHints.ShowTextHint(interactable.attachedToHand, m_SwitchToolAction, "Switch Tool");
                hintHand = interactable.attachedToHand;
            }
            else if (!shownDrawHint)
            {
                ControllerButtonHints.ShowButtonHint(interactable.attachedToHand, m_DrawAction);
                ControllerButtonHints.ShowTextHint(interactable.attachedToHand, m_DrawAction, "Draw");
                hintHand = interactable.attachedToHand;
            }
        }
        else if (hintHand != null)
        {
            ControllerButtonHints.HideAllButtonHints(hintHand);
            ControllerButtonHints.HideAllTextHints(hintHand);
            hintHand = null;
        }
    }

    public void TriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (interactable.attachedToHand == null) return;

        if (shownChangeToolHint && !shownDrawHint)
        {
            shownDrawHint = true;
            ControllerButtonHints.HideButtonHint(interactable.attachedToHand, m_DrawAction);
            ControllerButtonHints.HideTextHint(interactable.attachedToHand, m_DrawAction);
        }

        StartDraw();
    }

    public void TriggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (interactable.attachedToHand == null) return;

        StopDraw();
    }
    
    public void ButtonDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (interactable.attachedToHand == null) return;

        if (!shownChangeToolHint)
        {
            shownChangeToolHint = true;
            ControllerButtonHints.HideButtonHint(interactable.attachedToHand, m_SwitchToolAction);
            ControllerButtonHints.HideTextHint(interactable.attachedToHand, m_SwitchToolAction);
        }
        // cycle through attachments
        int count = attachments.childCount;
        if (currentAttachment == count - 1)
        {
            ToggleMeshRenderer(attachments.GetChild(currentAttachment), false);
            currentAttachment = -1;
            SetTool("none");
        }
        else
        {
            if (currentAttachment != -1)
                ToggleMeshRenderer(attachments.GetChild(currentAttachment), false);

            ++currentAttachment;
            ToggleMeshRenderer(attachments.GetChild(currentAttachment), true);

            SetTool(attachments.GetChild(currentAttachment).name);
        }
    }

    private void ToggleMeshRenderer(Transform obj, bool enabled)
    {
        obj.gameObject.GetComponentInChildren<MeshRenderer>().enabled = enabled;
    }

    private void SetTool(string toolName)
    {
        toolName = toolName.ToLower();
        if (toolName.Contains("none"))
            m_InputController.SetTool(InputController.Tool.None);
        else if (toolName.Contains("line"))
            m_InputController.SetTool(InputController.Tool.Line);
        else if (toolName.Contains("area"))
            m_InputController.SetTool(InputController.Tool.Area);
        else if (toolName.Contains("cube"))
            m_InputController.SetTool(InputController.Tool.Volume);
        else if (toolName.Contains("mesh"))
            m_InputController.SetTool(InputController.Tool.Mesh);
    }

    void StartDraw()
    {
        Debug.Log("State: start");
        m_InputController.Clicked();
        isTriggerDown = true;
    }

    void Drawing()
    {
        Debug.Log("State: drawing");
        m_InputController.Dragged();
    }

    void StopDraw()
    {
        Debug.Log("State: end");
        isTriggerDown = false;
        m_InputController.MouseUp();
    }
}
