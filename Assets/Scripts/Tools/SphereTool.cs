using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereTool : ToolType<DrawableSphere>
{
    public override Tool Name {
        get { return Tool.Sphere; }
    }

    protected override void OnAwake() {
        m_DrawableShape = GetComponent<DrawableSphere>();
    }

    public override void OnTriggerDown()
    {
         m_DrawableShape.StartDrawing(m_Pointer.transform.position);
    }
}
