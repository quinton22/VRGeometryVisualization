using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightUpOnCollision : MonoBehaviour
{

    public bool collision;
    private InputController mInputController;
    private bool Enabled = true;
    public bool wasEnabled = true;
    public MeshRenderer m_MeshRenderer;
    public Material m_Mat;
    // Start is called before the first frame update
    void Start()
    {
        mInputController = GameObject.Find("ToolController").GetComponent<InputController>();
        m_MeshRenderer = GetComponent<MeshRenderer>();
        m_Mat = m_MeshRenderer.material;

        collision = false;
    }

    // Update is called once per frame
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
        else if (enabled && !wasEnabled)
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

            if (m_Mat.GetFloat("_Mode") == 2)
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
        return (ct == InputController.Tool.Area && gameObject.name.Contains("Line")) ||
            (ct == InputController.Tool.Volume && transform.parent.gameObject.name.Contains("Area")) ||
            (ct == InputController.Tool.Volume && gameObject.name.Contains("Mesh")) ||
            (ct == InputController.Tool.Mesh && gameObject.name.Contains("Sphere"));
    }


}
