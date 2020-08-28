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

public abstract class ToolType<T> : MonoBehaviour where T : DrawableShape
{
    public GameObject m_Pointer;
    protected PointerController m_PointerController;
    protected bool m_SnapToGrid;

    protected virtual T m_DrawableShape {
        get;
        set;
    }
    public abstract Tool Name {
        get;
    }

    void Awake() {
        m_PointerController =  m_Pointer.GetComponent<PointerController>();
        m_SnapToGrid = FindObjectOfType<InputController>().m_SnapToGrid;

        OnAwake();
    }

    protected virtual void OnAwake() {}
    
    public abstract void OnTriggerDown();
    public virtual void OnTriggerHold()
    {
        m_DrawableShape.Drawing(m_Pointer.transform.position);
    }
    public virtual void OnTriggerUp()
    {
        m_PointerController.DecreaseSizeOfPointer();
        m_DrawableShape.StopDrawing(m_SnapToGrid);
    }
}
