using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// LightUpOnCollision
// Controls whether or not a shape lights up on collision
public class LightUpOnCollision : MonoBehaviour
{
    [System.NonSerialized]
    public bool collision = false;
    [System.NonSerialized]
    public MeshRenderer m_MeshRenderer;
    [System.NonSerialized]
    public Material m_Mat;
    private InputController mInputController;
    private bool Enabled = true;
    [System.NonSerialized]
    public bool wasEnabled = true;
    private PointerController pointerController;
    private DeleteTool deleteTool;
    [SerializeField]
    private bool transparent = false;
    private ShaderVariant shaderVariant;

    public abstract class ShaderVariant 
    {
        public const string KEYWORD = "_EMISSION";
        public Material material;
        public abstract bool Emission
        {
            get;
            set;
        }

        public static ShaderVariant Factory(bool transparent, Material _material)
        {
            ShaderVariant sv = transparent ? (ShaderVariant) new TransparentShaderVariant() : (ShaderVariant) new OpaqueShaderVariant();
            sv.material = _material;
            return sv;
        }
    }

    public class TransparentShaderVariant : ShaderVariant
    {
        public override bool Emission
        {
            get {
                return material.IsKeywordEnabled(KEYWORD);
            }
            set {
                if (value) material.EnableKeyword(KEYWORD);
                else material.DisableKeyword(KEYWORD);
            }
        }
    }

    public class OpaqueShaderVariant : ShaderVariant
    {
        public override bool Emission
        {
            get {
                return material.GetFloat(KEYWORD) == 1;
            }
            set {
                material.SetFloat(KEYWORD, value ? 1 : 0);
            }
        }
    }

    public void Start()
    {
        mInputController = FindObjectOfType<InputController>();
        pointerController = FindObjectOfType<PointerController>();
        deleteTool = FindObjectOfType<DeleteTool>();

        m_MeshRenderer = GetComponent<MeshRenderer>();
        m_Mat = m_MeshRenderer.material;
        shaderVariant = ShaderVariant.Factory(transparent, m_Mat);
    }

    void Update()
    {
        if (!Enabled && wasEnabled)
        {
            wasEnabled = false;

            if (shaderVariant.Emission)
            {
                shaderVariant.Emission = false;
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
            if (transparent && m_Mat.GetFloat("_Mode") == 2) // fade shader
            {
                m_Mat.color = Color.white;
            }

            if (!shaderVariant.Emission && checkCurrentTool())
            {
                shaderVariant.Emission = true;
                collision = true;
             
                pointerController.collidingObject = gameObject;
                
            }

        }
        else if (other.gameObject.name == "DeleteToolIntersector")
        {
            if (!shaderVariant.Emission)
            {
                shaderVariant.Emission = true;
                collision = true;
                deleteTool.intersectingObject = gameObject;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Pointer")
        {            
            if (transparent && m_Mat.GetFloat("_Mode") == 2) // fade shader
            {
                m_Mat.color = Color.clear;
            }

            if (shaderVariant.Emission)
            {
                shaderVariant.Emission = false;
                collision = false;

                pointerController.collidingObject = null;
            }
        }
        else if (other.gameObject.name == "DeleteToolIntersector")
        {
            if (shaderVariant.Emission)
            {
                shaderVariant.Emission = false;
                collision = false;
                deleteTool.intersectingObject = null;
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
