using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiTool : ToolType
{
    public override Tool Name {
        get { return Tool.Multi; }
    }

    private LineTool lineTool;
    private AreaTool areaTool;
    private VolumeTool volumeTool;
    private ToolType currentTool;

    protected override void OnAwake()
    {
        lineTool = FindObjectOfType<LineTool>();
        areaTool = FindObjectOfType<AreaTool>();
        volumeTool = FindObjectOfType<VolumeTool>();
        if (isToolEnabled)
            toolTypeList.Add(this);
    }

    public override void OnTriggerDown()
    {
        GameObject collObj = m_PointerController.collidingObject;
        if (collObj != null && collObj.GetComponent<ShapeType>().m_ShapeType == ShapeTypeEnum.Line) // draw area
        {
            currentTool = areaTool;
        }
        else if (collObj != null && collObj.GetComponent<ShapeType>().m_ShapeType == ShapeTypeEnum.Area) // draw volume
        {
            currentTool = volumeTool;
        }
        else // draw line
        {
            currentTool = lineTool;
        }
        currentTool.OnTriggerDown();
    }

    public override void OnTriggerHold()
    {
        currentTool.OnTriggerHold();
    }

    public override void OnTriggerUp()
    {
        currentTool.OnTriggerUp();
        currentTool = null;
    }
}
