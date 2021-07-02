using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereTool : ToolType
{
    public override Tool Name {
        get { return Tool.Sphere; }
    }

    protected override void OnAwake() {
        m_DrawableShape = GetComponent<DrawableSphere>();
        if (isToolEnabled)
            toolTypeList.Add(this);
    }

    public override void OnTriggerDown()
    {
        base.OnTriggerDown();
        m_DrawableShape.StartDrawing(m_Pointer.transform.position);
    }

    public override void OnTriggerHold()
    {
        base.OnTriggerHold();
    }
}
