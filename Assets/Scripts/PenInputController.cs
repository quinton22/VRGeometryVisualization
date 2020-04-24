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
    private Transform anchorTransform;
    //public bool isJointAttached = false;
    [SerializeField]
    [Tooltip("How long the collider takes to flash in seconds")]
    private float m_TotalLightUpCycleTime;
    private float currentCycleTime = 0;
    [SerializeField]
    private GameObject lightUpCollider;
    private Material lightUpColliderMaterial;
    private Rigidbody rigidbody;
    [SerializeField]
    private float m_BreakForce = Mathf.Infinity;

    void Start()
    {
        interactable = GetComponent<Interactable>();
        anchorTransform = transform.Find("AttachPoint");
        lightUpColliderMaterial = lightUpCollider.GetComponent<MeshRenderer>().material;
        if (!lightUpColliderMaterial.IsKeywordEnabled("_EMISSION"))
            lightUpColliderMaterial.EnableKeyword("_EMISSION");

        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (IsJointAttached() && fixedJoint.connectedBody == null)
        {
            //isJointAttached = false;
            Destroy(fixedJoint);
            m_InputController.SetTool(InputController.Tool.None);
        }

        if (interactable.attachedToHand != null)
        {
            //rigidbody.isKinematic = true;
            if (!IsJointAttached())
            {
                // light flash collider to show where to put a tool
                // Increase current time by deltaTime or reset to 0
                currentCycleTime += currentCycleTime < m_TotalLightUpCycleTime ? Time.deltaTime : -currentCycleTime;
            }
            else if (currentCycleTime != 0) // make sure collider is transparent if joint is attached
            {
                currentCycleTime = 0;
            }

            // use current cycle time to determine color of collider
            float lerpValue = (currentCycleTime > m_TotalLightUpCycleTime / 2
                ? m_TotalLightUpCycleTime - currentCycleTime
                : currentCycleTime)
                / (m_TotalLightUpCycleTime / 2);

            if (lightUpColliderMaterial.GetFloat("_Mode") == 2) // fade shader
            {
                lightUpColliderMaterial.color = Color.Lerp(Color.clear, Color.white, lerpValue);
            }

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
        else
        {
            //rigidbody.isKinematic = false;
            if (lightUpColliderMaterial.GetFloat("_Mode") == 2 && lightUpColliderMaterial.color != Color.clear) // fade shader
            {
                lightUpColliderMaterial.color = Color.clear;
            }
        }


        // TODO: remove
        // for debugging purposes, toggle the fixed joint break strength in real time
        if (IsJointAttached() && fixedJoint.connectedBody != null)
        {
            fixedJoint.breakForce = m_BreakForce;
        }
    }

    bool IsJointAttached()
    {
        return fixedJoint != null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (fixedJoint != null && fixedJoint.connectedBody != null) return;

        
        string name = other.gameObject.name.ToLower();
        if (name.Contains("line") || name.Contains("area") || name.Contains("cube") || name.Contains("mesh"))
        {
            fixedJoint = gameObject.AddComponent<FixedJoint>();
            Debug.Log($"Collider entered: {other.name}");

            fixedJoint.connectedBody = other.attachedRigidbody;
            fixedJoint.anchor = anchorTransform.localPosition;
            
            // TODO: figure out how to make the attachpoint on the obj line up with the anchor point
            // find offset of obj and attachpoint, anchor = obj.transform.pos + offset;
            Transform objAttachPoint = other.transform.Find("AttachPoint");
            if (objAttachPoint != null)
            {
                Vector3 offset = objAttachPoint.localPosition; // TODO: this is local offset not global
                other.transform.position = anchorTransform.position - offset;
            }
            other.transform.rotation = anchorTransform.rotation;

            //isJointAttached = true;

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
