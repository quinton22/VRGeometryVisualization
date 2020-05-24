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
    private MeshRenderer meshRenderer;
    private Material material;

    //public Vector3 _scale;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.sharedMaterial;
        UpdateGridScale();
        GlobalGridScale.AddScaleListener(UpdateGridScale);
        InitializeMaterial();
    }
    
    void Update()
    {  
        SetMaterialScale();
    }

    void OnValidate()
    {
        gridScale = Mathf.Max(0, gridScale);
        gridLineThickness = Mathf.Max(0, gridLineThickness);
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.sharedMaterial;

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
    }

    void SetMaterialScale()
    {
        Vector3 scale = m_ShapeType == ShapeType.Area ? transform.parent.localScale : transform.localScale;
        material.SetVector("_Scale", new Vector4(scale.x, scale.y, scale.z, 1));
    }
}
