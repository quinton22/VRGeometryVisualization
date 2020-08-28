using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiTool : ToolType<DrawableShape>
{
    public override Tool Name {
        get { return Tool.Multi; }
    }

    private LineTool lineTool;
    private AreaTool areaTool;
    private VolumeTool volumeTool;

    protected override void OnAwake()
    {
        lineTool = FindObjectOfType<LineTool>();
        areaTool = FindObjectOfType<AreaTool>();
        volumeTool = FindObjectOfType<VolumeTool>();
    }

    public override void OnTriggerDown()
    {
        GameObject collObj = m_PointerController.collidingObject;
        if (collObj != null && collObj.GetComponent<ShapeType>().m_ShapeType == ShapeTypeEnum.Line) // draw area
        {
            areaTool.OnTriggerDown();
        }
        else if (collObj != null && collObj.GetComponent<ShapeType>().m_ShapeType == ShapeTypeEnum.Area) // draw volume
        {
            volumeTool.OnTriggerDown();
        }
        else // draw line
        {
            lineTool.OnTriggerDown();
        }
    }
}
