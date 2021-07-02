using UnityEngine;

public enum Tool
{
    None,
    Line,
    Area,
    Volume,
    Sphere,
    Polygon,
    Multi,
    Delete
}

// public abstract class ToolType : MonoBehaviour {

//     public virtual void OnTriggerDown() {}
//     public virtual void OnTriggerHold() {}
//     public virtual void OnTriggerUp() {}
// }

public abstract class ToolType : MonoBehaviour
{
    public abstract Tool Name {
        get;
    }
    public bool isToolEnabled = true;
    protected GameObject m_Pointer;
    protected PointerController m_PointerController;
    protected bool m_SnapToGrid;
    protected ToolTypeList toolTypeList;

    protected DrawableShape m_DrawableShape;
    protected static Plane? restrictedPlane;
    protected static NewInputController.RestrictedRect restrictedRect;

    void Awake() {
        m_Pointer = GameObject.Find("Pointer");
        m_PointerController =  m_Pointer.GetComponent<PointerController>();
        m_SnapToGrid = FindObjectOfType<InputController>().m_SnapToGrid;
        toolTypeList = FindObjectOfType<ToolTypeList>();

        OnAwake();
    }

    protected virtual void OnAwake() {}
    
    public virtual void OnTriggerDown()
    {
    }
    public virtual void OnTriggerHold()
    {
        if (m_DrawableShape)
        {
            m_DrawableShape.Drawing(m_Pointer.transform.position);
        }
    }
    public virtual void OnTriggerUp()
    {
        if (m_DrawableShape)
        {
            m_PointerController.DecreaseSizeOfPointer();
            m_DrawableShape.StopDrawing(m_SnapToGrid);
        }
    }

    public virtual void RestrictToPlane(Plane plane, NewInputController.RestrictedRect withBoundingBox = null)
    {
        restrictedPlane = plane;
        restrictedRect = withBoundingBox;
    }

    public void UnrestrictFromPlane()
    {
        restrictedPlane = null;
        restrictedRect = null;
    }
}
