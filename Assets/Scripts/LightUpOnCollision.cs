using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// LightUpOnCollision
// Controls whether or not a shape lights up on collision
public class LightUpOnCollision : MonoBehaviour
{

    public bool collision;
    private InputController mInputController;
    private bool Enabled = true;
    public bool wasEnabled = true;
    public MeshRenderer m_MeshRenderer;
    public Material m_Mat;
    void Start()
    {
        mInputController = GameObject.Find("ToolController").GetComponent<InputController>();
        m_MeshRenderer = GetComponent<MeshRenderer>();
        m_Mat = m_MeshRenderer.material;

        collision = false;
    }

    void Update()
    {
        if (!Enabled && wasEnabled)
        {
            wasEnabled = false;

            if (m_Mat.IsKeywordEnabled("_EMISSION"))
            {
                m_Mat.DisableKeyword("_EMISSION");
                collision = false;
            }

        }
        else if (Enabled && !wasEnabled)
        {
            wasEnabled = true;
        }
    }

    public void SetEnabled(bool e)
    {
        Enabled = e;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!Enabled) return;

        if (other.gameObject.name == "Pointer")
        {
            bool enabled = m_Mat.IsKeywordEnabled("_EMISSION");

            if (m_Mat.GetFloat("_Mode") == 2) // fade shader
            {
                m_Mat.color = Color.white;
            }

            if (!enabled && checkCurrentTool())
            {
                m_Mat.EnableKeyword("_EMISSION");
                collision = true;
             
                other.gameObject.GetComponent<PointerController>().collidingObject = gameObject;
                
            }

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Pointer")
        {
            bool enabled = m_Mat.IsKeywordEnabled("_EMISSION");
            
            if (m_Mat.GetFloat("_Mode") == 2) // fade shader
            {
                m_Mat.color = Color.clear;
            }

            if (enabled)
            {
                m_Mat.DisableKeyword("_EMISSION");
                collision = false;

                other.gameObject.GetComponent<PointerController>().collidingObject = null;
            }

        }
    }

    bool checkCurrentTool()
    {
        InputController.Tool ct = mInputController.m_CurrentTool;
        return (ct == InputController.Tool.None) ||
            (ct == InputController.Tool.Area && gameObject.name.Contains("Line")) ||
            (ct == InputController.Tool.Volume && transform.parent.gameObject.name.Contains("Area")) ||
            (ct == InputController.Tool.Volume && gameObject.name.Contains("Mesh")) ||
            (ct == InputController.Tool.Mesh && gameObject.name.Contains("Sphere"));
    }


}
