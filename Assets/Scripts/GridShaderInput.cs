using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridShaderInput : MonoBehaviour
{
    private enum ShapeType
    {
        Line,
        Area,
        Volume,
        Mesh
    }
    [SerializeField]
    private ShapeType m_ShapeType;
    [SerializeField]
    private float gridScale = 1;
    [SerializeField]
    private float gridLineThickness = 0.02f;
    private Renderer _renderer;
    private Material material;
    private MaterialPropertyBlock propBlock;

    private bool _gridOn = true;

    //public Vector3 _scale;

    void Awake()
    {
        propBlock = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();
        material = _renderer.material;
        UpdateGridScale();
        GlobalGridScale.AddScaleListener(UpdateGridScale);
        InitializeMaterial();

        _gridOn = GlobalGridScale.Instance.GridOn;
    }
    
    void Update()
    {  
        SetMaterialScale();

        if (GlobalGridScale.Instance.GridOn != _gridOn) {
            _gridOn = GlobalGridScale.Instance.GridOn;
            UpdateGridOn();
        }
    }

    void OnValidate()
    {
        gridScale = Mathf.Max(0, gridScale);
        gridLineThickness = Mathf.Max(0, gridLineThickness);
        _renderer = GetComponent<Renderer>();
        material = _renderer.sharedMaterial;

        // if (m_ShapeType == ShapeType.Area)
        //     transform.parent.localScale = _scale;
        // else
        //     transform.localScale = _scale;
            
        InitializeMaterial();
        material.SetFloat("_GridScale", gridScale);
        SetMaterialScale();
    }

    void InitializeMaterial()
    {
        material.SetInt("_ShapeType", (int) m_ShapeType);
        material.SetFloat("_GridLineThickness", gridLineThickness);
    }

    void UpdateGridScale()
    {
        gridScale = GlobalGridScale.Instance.GridScale;
        material.SetFloat("_GridScale", gridScale);
        gridLineThickness = 4 * 0.02f / gridScale;
        material.SetFloat("GridLineThickness", gridLineThickness);
    }

    void UpdateGridOn()
    {
        material.SetFloat("_ShadeGrid", _gridOn ? 1 : 0);
    }

    void SetMaterialScale()
    {
        if (propBlock == null)
            propBlock = new MaterialPropertyBlock();
        
        Vector3 scale = m_ShapeType == ShapeType.Area ? transform.parent.localScale : transform.localScale;
        _renderer.GetPropertyBlock(propBlock);
        propBlock.SetVector("_Scale", new Vector4(scale.x, scale.y, scale.z, 1));
        _renderer.SetPropertyBlock(propBlock);
    }
}
