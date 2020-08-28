using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTool : ToolType<DrawableLine>
{
    public override Tool Name {
        get { return Tool.Line; }
    }

    void Awake() {
        m_DrawableShape = GetComponent<DrawableLine>();
    }

    public override void OnTriggerDown()
    {
        m_PointerController.IncreaseSizeOfPointer();

        // TODO:
        m_DrawableShape.StartDrawing(m_Pointer.transform.position);
    }
}
