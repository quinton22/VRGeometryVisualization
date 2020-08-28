using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

// InputController class
// Controls the tools and geometry manipulation given user input
public class InputController : MonoBehaviour
{
    public enum Tool
    {
        None,
        Line,
        Area,
        Volume,
        Sphere,
        Mesh
    }

    // New stuff
    // ================================================
    private DrawableLine m_DrawableLine;
    private DrawableArea m_DrawableArea;
    private DrawableVolume m_DrawableVolume;
    private DrawableSphere m_DrawableSphere;
    private DrawablePolygon m_DrawablePolygon;



    public bool m_SnapToGrid = true;






    // ================================================

    [System.NonSerialized]
    public Tool m_CurrentTool;
    // what to divide 1 unit up into
    public float m_ScaleDivision;
    private Text m_ToolText;
    [HideInInspector]
    public Tool drawing;
    private Vector3 initialPosition;
    private Vector3 currentPosition;
    [SerializeField] private GameObject m_Shape;
    private GameObject m_Line;
    private GameObject m_Area;
    private GameObject m_Volume;
    private GameObject m_Mesh;
    private GameObject m_LineCopy;
    private GameObject m_AreaCopy;
    private GameObject m_VolumeCopy;
    private GameObject m_MeshCopy;
    private MeshCreatorController m_MeshCreatorController;
    private GameObject m_LineForArea;
    private GameObject m_AreaForVolume;
    private GameObject m_MeshForVolume;
    private Vector3 m_VolumeForward;
    [SerializeField] private GameObject m_Parent;
    [SerializeField] private GameObject m_Pointer;
    private PointerController m_PointerController;
    private PenInputController penInput;
    private Vector3 initialPenPosition;
    private DeleteToolOld deleteTool;
    private Vector3 initialDeleteToolPosition;
    private IntroductionScript mIntroductionScript;

    // Start is called before the first frame update
    void Start()
    {

        m_DrawableLine = GetComponent<DrawableLine>();
        m_DrawableArea = GetComponent<DrawableArea>();
        m_DrawableVolume = GetComponent<DrawableVolume>();
        m_DrawableSphere = GetComponent<DrawableSphere>();
        m_DrawablePolygon = GetComponent<DrawablePolygon>();

        // For introduction
        m_DrawableVolume.AddListener(() => { mIntroductionScript.CheckVolume(m_DrawableVolume.m_LastCreatedShape.transform, DrawableVolume.m_SubdivisionScale);},
            DrawableVolume.ListenerType.PostFinish);


        m_CurrentTool = Tool.None;
//        m_ToolText = GameObject.Find("ActiveToolText").GetComponent<Text>();

        mIntroductionScript = FindObjectOfType<IntroductionScript>();

        UpdateGridScale();
        GlobalGridScale.AddScaleListener(UpdateGridScale);

        m_Line = m_Shape.transform.Find("Line").gameObject;
        m_Area = m_Shape.transform.Find("Area").gameObject;
        m_Volume = m_Shape.transform.Find("Volume").gameObject;
        m_Mesh = m_Shape.transform.Find("Mesh").gameObject;

        m_PointerController = m_Pointer.GetComponent<PointerController>();

        penInput = FindObjectOfType<PenInputController>();
        initialPenPosition = penInput.transform.position;

        deleteTool = FindObjectOfType<DeleteToolOld>();
        initialDeleteToolPosition = deleteTool.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
    #if UNITY_EDITOR
        if (Input.GetKeyDown("l"))
        {
            CyclePenTools(Tool.Line);
        }
        else if (Input.GetKeyDown("r"))
        {
            // area tool
            CyclePenTools(Tool.Area);
        }
        else if (Input.GetKeyDown("v"))
        {
            // volume tool
            CyclePenTools(Tool.Volume);
        }
        else if (Input.GetKeyDown("m"))
        {
            // mesh tool
            CyclePenTools(Tool.Mesh);
        }
        else if (Input.GetKeyDown("n"))
        {
            // none
            CyclePenTools(Tool.None);
        }
        else if (Input.GetKeyDown("x"))
        {
            deleteTool.Delete();
        }

        if (Input.GetKeyDown("space"))
        {
            Clicked();
        }

        if (Input.GetKey("space"))
        {
            Dragged();
        }

        if (Input.GetKeyUp("space"))
        {
            MouseUp();
        }
    #endif
        if (drawing == Tool.Mesh)
        {            
            // don't need to hold and click to create line
            //currentPosition = m_Pointer.transform.position;
            DrawMesh();
        }
    }

    public void ResetPenPosition()
    {
        penInput.transform.position = initialPenPosition;
        penInput.transform.rotation = Quaternion.identity;
        deleteTool.transform.position = initialDeleteToolPosition;
        deleteTool.transform.rotation = Quaternion.identity;
    }

    public void DeleteEverything()
    {
        for (int i = 0; i < m_Parent.transform.childCount; ++i)
        {
            Destroy(m_Parent.transform.GetChild(i).gameObject);
        }
    }

    private void UpdateGridScale()
    {
        m_ScaleDivision = (float) GlobalGridScale.Instance.GridScale;
    }

    private void CyclePenTools(Tool targetTool)
    {
        while (m_CurrentTool != targetTool)
        {
            penInput.NextAttachment();
        }
    }

    public void SetTool(Tool tool)
    {
        m_CurrentTool = tool;
    }

    /// Takes the current tool (mCurrentTool) and sets the tool text
    /// to represent that tool
    void SetToolText()
    {
        // set tool text on screen so user knows what tool is being used
        m_ToolText.text = "Active Tool: ";
        switch(m_CurrentTool)
        {
            case Tool.None:
                m_ToolText.text += "None";
                break;
            case Tool.Line:
                m_ToolText.text += "Line";
                break;
            case Tool.Area:
                m_ToolText.text += "Area";
                break;
            case Tool.Volume:
                m_ToolText.text += "Volume";
                break;
            case Tool.Mesh:
                m_ToolText.text += "Mesh";
                break;
            default:
                m_ToolText.text = "Error!";
                break;
        }
    }

    public void Clicked() 
    {
        // initialize a move
        switch(m_CurrentTool)
        {
            case Tool.Line:
                m_PointerController.IncreaseSizeOfPointer();

                // draw point ?
                drawing = Tool.Line;


                // TODO:
                m_DrawableLine.StartDrawing(m_Pointer.transform.position);

                break;
            case Tool.Area:
                if (m_PointerController.collidingObject != null && m_PointerController.collidingObject.name.Contains("Line"))
                {   
                    m_PointerController.IncreaseSizeOfPointer();

                    m_LineForArea = m_PointerController.collidingObject;
                    drawing = Tool.Area;

                    // TODO: change back
                    m_DrawableArea.m_UpVec = m_LineForArea.transform.up;
                    m_DrawableArea.m_YScale = m_LineForArea.transform.localScale.y;
                    m_DrawableArea.StartDrawing(m_LineForArea.transform.position);
                    
                    Dragged(); // TODO: do we need? if so add to post start
                }
                break;
            case Tool.Volume:
                if (m_PointerController.collidingObject != null)
                {
                    m_PointerController.IncreaseSizeOfPointer();

                    if (m_PointerController.collidingObject.transform.parent.gameObject.name.Contains("Area"))
                    {
                        m_AreaForVolume = m_PointerController.collidingObject.transform.parent.gameObject;
                        drawing = Tool.Volume;
                        // TODO: change back
                        m_DrawableVolume.m_AreaLocalRotation = m_AreaForVolume.transform.localRotation;
                        m_DrawableVolume.m_AreaLocalScale = m_AreaForVolume.transform.localScale;
                        m_DrawableVolume.m_AreaForwardDir = m_AreaForVolume.transform.forward;
                        m_DrawableVolume.StartDrawing(m_AreaForVolume.transform.position);
                    }
                    else if (m_PointerController.collidingObject.transform.name.Contains("Mesh"))
                    {
                        // start extruding mesh
                        m_MeshForVolume = m_PointerController.collidingObject;
                        drawing = Tool.Volume;
                        initialPosition = m_MeshForVolume.transform.position;
                        
                        m_MeshCreatorController = m_MeshForVolume.GetComponent<MeshCreatorController>();
                    }

                    Dragged();
                } 
                break;
            case Tool.Sphere:
                drawing = Tool.Sphere;
                m_DrawableSphere.StartDrawing(m_Pointer.transform.position);
                break;
            case Tool.Mesh:
                
                
                // TODO
                // maybe create a plane that we draw on with a grid (could make the grid lines variable in spacing)
                break;
            case Tool.None:
            default:
                drawing = Tool.None;
                break;
        }
    }

    public void Dragged()
    {
        // drag out
        if (m_CurrentTool == Tool.Mesh || m_CurrentTool == Tool.None) return;
        
        currentPosition = m_Pointer.transform.position;

        DrawableShape drawableShape;
        if (m_CurrentTool == Tool.Line)
            drawableShape = m_DrawableLine;
        else if (m_CurrentTool == Tool.Area)
            drawableShape = m_DrawableArea;
        else if (m_CurrentTool == Tool.Sphere)
            drawableShape = m_DrawableSphere;
        else
            drawableShape = m_DrawableVolume;

        drawableShape.Drawing(currentPosition);
    }

    public void MouseUp()
    {
        // finish drawing
        if (drawing == Tool.None && m_CurrentTool != Tool.Mesh) return;

        if (m_CurrentTool != Tool.Mesh && (m_CurrentTool != Tool.Volume || m_MeshForVolume == null))
        {
            drawing = Tool.None;

            initialPosition = Vector3.zero;
            currentPosition = Vector3.zero;

            m_PointerController.DecreaseSizeOfPointer();
        }
        
        // drag out
        switch (m_CurrentTool)
        {
            case Tool.Line:
                // TODO: change back
                m_DrawableLine.StopDrawing(m_SnapToGrid);
                break;
            case Tool.Area:
                // TODO: change back
                m_DrawableArea.StopDrawing(m_SnapToGrid);

                Destroy(m_LineForArea);
                break;
            case Tool.Volume:
                // if (m_VolumeCopy != null)
                // {
                if (m_MeshForVolume == null)
                {
                    // TODO: change back
                    m_DrawableVolume.StopDrawing(m_SnapToGrid);

                    Destroy(m_AreaForVolume);
                }
                else
                {
                    m_PointerController.DecreaseSizeOfPointer();

                    m_MeshForVolume.GetComponent<LightUpOnCollision>().SetEnabled(false);

                    if (m_MeshCreatorController.m_SnapToGrid)
                    {
                        DrawVolume(true);
                    }

                    m_MeshForVolume = null;
                    m_MeshCreatorController.FinishMesh();
                    m_PointerController.collidingObject = null;
                    currentPosition = Vector3.zero;
                    initialPosition = Vector3.zero;
                    drawing = Tool.None;

                }
                break;
            case Tool.Sphere:
                m_DrawableSphere.StopDrawing(m_SnapToGrid);
                break;
            case Tool.Mesh:
                if (drawing != Tool.Mesh) // start drawing mesh
                {
                    m_PointerController.IncreaseSizeOfPointer();

                    drawing = Tool.Mesh;
                    initialPosition = m_Pointer.transform.position;
                    m_MeshCopy = UnityEngine.Object.Instantiate(m_Mesh, initialPosition, Quaternion.identity, m_Parent.transform);
                    m_MeshCreatorController = m_MeshCopy.GetComponent<MeshCreatorController>();
                    
                    m_LineCopy = m_MeshCreatorController.AddLine(initialPosition);
                    DrawMesh();
                }
                else
                {
                    // want to end current line and start new line unless we are back at the starting position
                    // if m_MeshCreatorController.m_SnapToGrid is true, then the line should snap to a whole number plus the initial pos
                    if (m_MeshCreatorController.m_SnapToGrid)
                    {
                        // last currentPosition should snap to the first initial position plus or minus a whole number
                        DrawMesh(true);
                        initialPosition = currentPosition;
                    }
                    else
                    {
                        initialPosition = m_Pointer.transform.position;
                    }

                    GameObject originalLine = m_MeshCreatorController.GetLine(0);
                    if (originalLine.transform.Find("Sphere").gameObject.GetComponent<LightUpOnCollision>().collision)
                    {
                        m_PointerController.DecreaseSizeOfPointer();

                        // end mesh
                        m_LineCopy = null;
                        drawing = Tool.None;

                        m_MeshCreatorController.CreateMesh();
                        m_MeshCreatorController.DeleteLines();

                        m_PointerController.UnrestrictPointerToInsideRays();
                        m_PointerController.isCentered = true;
                    }
                    else
                    {
                        // new line
                        m_LineCopy = m_MeshCreatorController.AddLine(initialPosition);
                        DrawMesh();
                    }
                }
                break;
            case Tool.None:
            default:
                break;
        }
        
    }

    void DrawVolume(bool snapToGrid=false)
    {
        if (m_VolumeCopy != null)
        {
            m_VolumeCopy.transform.localRotation = m_AreaForVolume.transform.localRotation;
            m_VolumeCopy.transform.localScale = m_AreaForVolume.transform.localScale;

            m_VolumeForward = Vector3.Project((currentPosition - initialPosition), m_VolumeCopy.transform.forward);

            m_VolumeCopy.transform.localScale = new Vector3(
                m_VolumeCopy.transform.localScale.x,
                m_VolumeCopy.transform.localScale.y,
                m_VolumeForward.magnitude
            );

            m_VolumeCopy.transform.position = initialPosition + m_VolumeForward.normalized * m_VolumeCopy.transform.localScale.z / 2;

            m_VolumeCopy.GetComponent<VolumeForwardController>().dir = Vector3.Project((currentPosition - initialPosition), -m_AreaForVolume.transform.forward);
        }
        else if (m_MeshForVolume != null)
        {
            Vector3 updatedValue = Vector3.Project((currentPosition - initialPosition), m_MeshCreatorController.GetNorm());
            if (snapToGrid)
            {
                float scale = m_ScaleDivision;
                updatedValue *= scale;
                // TODO: this should be rounded based on the direction it is going in
                // we want to round the magnitude to the nearest whole number
                float magnitudeToRoundTo = Mathf.Round(updatedValue.magnitude);
                Vector3 updatedValue2 = updatedValue.normalized * magnitudeToRoundTo;

                if (updatedValue2.magnitude == 0)
                    updatedValue = updatedValue.normalized;
                else
                    updatedValue = updatedValue2;

                updatedValue /= scale;
            }
            m_MeshCreatorController.UpdateMesh(updatedValue);
            
        }
    }

    void DrawMesh(bool snapToGrid=false)
    {
        if (m_MeshCopy != null && m_LineCopy != null)
        {
            if (m_MeshCreatorController.m_Vertices.Count >= 3 && m_PointerController.isCentered)
            {
                // current position should be in the same plane as the first 3 vertices
                m_PointerController.SetPointerPlane(
                    m_MeshCreatorController.GetVertexWithWorldCoord(0),
                    m_MeshCreatorController.GetVertexWithWorldCoord(1),
                    m_MeshCreatorController.GetVertexWithWorldCoord(2)
                );

                m_PointerController.isCentered = false;
            }
            
            if (!snapToGrid)
            {
                currentPosition = m_Pointer.transform.position;
            }
            else if (!m_PointerController.isCentered) // snapping to the grid and not one of the first 3 points
            {
                // TODO: needs to be some scale here so as not to round to whole numbers (divided by 2)
                float scale = m_ScaleDivision;

                currentPosition = m_PointerController.GetGridPointFromPlane(m_Pointer.transform.position, scale);
            }
            else if (m_PointerController.isCentered && m_MeshCreatorController.m_Vertices.Count == 2)
            {
                // TODO: needs to be some scale here so as not to round to whole numbers (divided by 2)
                float scale = m_ScaleDivision;

                m_PointerController.SetPointerPlane(
                    m_MeshCreatorController.GetVertexWithWorldCoord(0),
                    m_MeshCreatorController.GetVertexWithWorldCoord(1),
                    currentPosition
                );

                m_PointerController.isCentered = false;

                currentPosition = m_PointerController.GetGridPointFromPlane(m_Pointer.transform.position, scale);
            }

            if (!m_PointerController.isCentered)
            {
                // make sure mesh is convex
                // by ensuring that the pointer is within the ray created by the 2nd to last vertex
                // and the last vertex and the ray created by the last vertex and the first vertex
                int count = m_MeshCreatorController.m_Vertices.Count;
                Vector3 vertexN = m_MeshCreatorController.GetVertexWithWorldCoord(count-1);
                Vector3 vertexNMinus1 = m_MeshCreatorController.GetVertexWithWorldCoord(count-2);
                Vector3 vertex1 = m_MeshCreatorController.GetVertexWithWorldCoord(0);


                Vector3 A = vertexN - vertexNMinus1;
                Vector3 B = vertex1 - vertexN;

                m_PointerController.RestrictPointerToInsideRays(vertexN, A, B);

            }

            m_LineCopy.transform.position = (initialPosition + currentPosition) / 2;
            m_LineCopy.transform.localScale = new Vector3(
                m_LineCopy.transform.localScale.x,
                (initialPosition - currentPosition).magnitude / 2,
                m_LineCopy.transform.localScale.z
            );

            if (snapToGrid && m_PointerController.isCentered)
            {
                float sd = m_ScaleDivision * 2;
                // round to nearest (sub)unit
                Vector3 scaleL = m_LineCopy.transform.localScale;
                scaleL.y *= sd;
                if (scaleL.y < 1)
                {
                    scaleL.y = 1;
                }
                else
                {
                    scaleL.y = Mathf.Round(scaleL.y);
                }
                scaleL.y /= sd;

                float deltaY = scaleL.y - m_LineCopy.transform.localScale.y;

                m_LineCopy.transform.localScale = scaleL;

                // move position to adjust for change of size
                Vector3 posL = m_LineCopy.transform.position;
                posL += m_LineCopy.transform.up * deltaY;
                m_LineCopy.transform.position = posL;

                currentPosition = m_LineCopy.transform.position + m_LineCopy.transform.up * m_LineCopy.transform.localScale.y;

            }

            if (currentPosition - initialPosition == Vector3.zero) return;
            m_LineCopy.transform.localRotation = Quaternion.LookRotation((currentPosition - initialPosition).normalized);
            m_LineCopy.transform.Rotate(90, 0, 0);
        }
    }
}
