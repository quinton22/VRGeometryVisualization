using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InputController : MonoBehaviour
{
    public enum Tool
    {
        None,
        Line,
        Area,
        Volume
    }

    public Tool m_CurrentTool;
    private Text m_ToolText;
    private Tool drawing;
    private Vector3 initialPosition;
    private Vector3 currentPosition;
    [SerializeField] private GameObject m_Shape;
    private GameObject m_Line;
    private GameObject m_Area;
    private GameObject m_Volume;
    private GameObject m_LineCopy;
    private GameObject m_AreaCopy;
    private GameObject m_VolumeCopy;
    private GameObject m_LineForArea;
    private GameObject m_AreaForVolume;
    [SerializeField] private GameObject m_Parent;
    [SerializeField] private GameObject m_Pointer;
    private PointerController m_PointerController;

    // Start is called before the first frame update
    void Start()
    {
        m_CurrentTool = Tool.None;
        m_ToolText = GameObject.Find("ActiveToolText").GetComponent<Text>();

        m_Line = m_Shape.transform.Find("Line").gameObject;
        m_Area = m_Shape.transform.Find("Area").gameObject;
        m_Volume = m_Shape.transform.Find("Volume").gameObject;

        m_PointerController = m_Pointer.GetComponent<PointerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("l"))
        {
            // line tool
            m_CurrentTool = Tool.Line;

            // opt to call SetToolText() here to mitigate the number of times
            // we update the text field
            SetToolText();
        }
        else if (Input.GetKeyDown("r"))
        {
            // area tool
            m_CurrentTool = Tool.Area;
            SetToolText();
        }
        else if (Input.GetKeyDown("v"))
        {
            // volume tool
            m_CurrentTool = Tool.Volume;
            SetToolText();
        }
        else if (Input.GetKeyDown("n"))
        {
            // none
            m_CurrentTool = Tool.None;
            SetToolText();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Clicked();
        }

        if (Input.GetMouseButton(0))
        {
            Dragged();
        }

        if (Input.GetMouseButtonUp(0))
        {
            MouseUp();
        }
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
            default:
                m_ToolText.text = "Error!";
                break;
        }
    }

    void Clicked() 
    {
        // initialize a move
        switch(m_CurrentTool)
        {
            case Tool.Line:
                // draw point ?
                drawing = Tool.Line;
                initialPosition = m_Pointer.transform.position;
                m_LineCopy = UnityEngine.Object.Instantiate(m_Line, initialPosition, Quaternion.identity, m_Parent.transform);
                break;
            case Tool.Area:
                if (m_PointerController.collidingObject != null && m_PointerController.collidingObject.name.Contains("Line"))
                {   
                    m_LineForArea = m_PointerController.collidingObject;
                    drawing = Tool.Area;
                    initialPosition = m_LineForArea.transform.position;
                    m_AreaCopy = UnityEngine.Object.Instantiate(m_Area, initialPosition, Quaternion.identity, m_Parent.transform);
                    Dragged();
                }
                break;
            case Tool.Volume:
                if (m_PointerController.collidingObject != null && m_PointerController.collidingObject.transform.parent.gameObject.name.Contains("Area"))
                {
                    m_AreaForVolume = m_PointerController.collidingObject.transform.parent.gameObject;
                    drawing = Tool.Volume;
                    initialPosition = m_AreaForVolume.transform.position;
                    m_VolumeCopy = UnityEngine.Object.Instantiate(m_Volume, initialPosition, Quaternion.identity, m_Parent.transform);
                    Dragged();
                }
                break;
            case Tool.None:
            default:
                drawing = Tool.None;
                break;
        }
    }

    void Dragged()
    {
        // drag out
        switch (m_CurrentTool)
        {
            case Tool.Line:
                currentPosition = m_Pointer.transform.position;
                DrawLine();
                break;
            case Tool.Area:
                currentPosition = m_Pointer.transform.position;
                DrawArea();
                break;
            case Tool.Volume:
                currentPosition = m_Pointer.transform.position;
                DrawVolume();
                break;
            case Tool.None:
            default:
                break;
        }
    }

    void MouseUp()
    {
        // finish drawing
        if (drawing != Tool.None)
        {
            drawing = Tool.None;

            initialPosition = Vector3.zero;
            currentPosition = Vector3.zero;
            // drag out
            switch (m_CurrentTool)
            {
                case Tool.Line:
                    m_LineCopy = null;
                    break;
                case Tool.Area:
                    m_AreaCopy = null;
                    Destroy(m_LineForArea);
                    break;
                case Tool.Volume:
                    m_VolumeCopy = null;
                    //Destroy(m_AreaForVolume);
                    break;
                case Tool.None:
                default:
                    break;
            }
        }
    }


    void DrawLine()
    {
        if (m_LineCopy != null)
        {
            m_LineCopy.transform.position = (initialPosition + currentPosition) / 2;
            m_LineCopy.transform.localScale = new Vector3(
                m_LineCopy.transform.localScale.x, 
                (initialPosition - currentPosition).magnitude / 2,
                m_LineCopy.transform.localScale.z);
            m_LineCopy.transform.localRotation = Quaternion.LookRotation((currentPosition - initialPosition).normalized);
            m_LineCopy.transform.Rotate(90, 0, 0);
        }
    }

    void DrawArea()
    {
        if (m_AreaCopy != null)
        {

            // set rotation of area so it is locked in the axis that is represented by the line
            Vector3 upVec = m_LineForArea.transform.up;
            Vector3 line_x = Vector3.Cross((currentPosition - initialPosition).normalized, upVec);            
            m_AreaCopy.transform.localRotation = Quaternion.LookRotation(line_x, upVec).normalized;

            m_AreaCopy.transform.localScale = new Vector3(
                Vector3.Project((currentPosition - initialPosition), m_AreaCopy.transform.right).magnitude,
                m_LineForArea.transform.localScale.y * 2,
                1
            );

            m_AreaCopy.transform.position = initialPosition + m_AreaCopy.transform.right * m_AreaCopy.transform.localScale.x / 2;
        }
    }
    
    void DrawVolume()
    {
        if (m_VolumeCopy != null)
        {
            m_VolumeCopy.transform.localRotation = m_AreaForVolume.transform.localRotation;
            m_VolumeCopy.transform.localScale = m_AreaForVolume.transform.localScale;

            Vector3 proj = Vector3.Project((currentPosition - initialPosition), m_VolumeCopy.transform.forward);

            m_VolumeCopy.transform.localScale = new Vector3(
                m_VolumeCopy.transform.localScale.x,
                m_VolumeCopy.transform.localScale.y,
                proj.magnitude
            );

            Debug.Log($"cur-init: {currentPosition - initialPosition}");
            Debug.Log($"normal: {(currentPosition - initialPosition).normalized}");
            Debug.Log($"mag: {(currentPosition - initialPosition).magnitude}");
            Debug.Log($"normal mag: {(currentPosition - initialPosition).normalized.magnitude}");

            m_VolumeCopy.transform.position = initialPosition + proj.normalized * m_VolumeCopy.transform.localScale.z / 2;
        }
    }
}