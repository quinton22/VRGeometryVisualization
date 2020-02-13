using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightUpOnCollision : MonoBehaviour
{

    public bool collision;
    private InputController mInputController;
    // Start is called before the first frame update
    void Start()
    {
        mInputController = GameObject.Find("ToolController").GetComponent<InputController>();
        Debug.Log(mInputController != null);
        collision = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Pointer")
        {
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            Material mat = meshRenderer.material;
            bool enabled = mat.IsKeywordEnabled("_EMISSION");

            if (!enabled && checkCurrentTool())
            {
                mat.EnableKeyword("_EMISSION");
                collision = true;
             
                other.gameObject.GetComponent<PointerController>().collidingObject = gameObject;
                
            }

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Pointer")
        {
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            Material mat = meshRenderer.material;
            bool enabled = mat.IsKeywordEnabled("_EMISSION");

            if (enabled)
            {
                mat.DisableKeyword("_EMISSION");
                collision = false;

                other.gameObject.GetComponent<PointerController>().collidingObject = null;
            }

        }
    }

    bool checkCurrentTool()
    {
        InputController.Tool ct = mInputController.m_CurrentTool;
        return (ct == InputController.Tool.Area && gameObject.name.Contains("Line")) || (ct == InputController.Tool.Volume && transform.parent.gameObject.name.Contains("Area"));
    }


}
