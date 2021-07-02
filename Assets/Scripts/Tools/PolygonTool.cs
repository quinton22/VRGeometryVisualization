using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonTool : ToolType
{
    public override Tool Name {
        get { return Tool.Polygon; }
    }

    protected override void OnAwake() {
        if (isToolEnabled)
            toolTypeList.Add(this);
        m_DrawableShape = GetComponent<DrawablePolygon>();
    }

    public override void OnTriggerDown()
    {

    }
    public override void OnTriggerHold()
    {

    }
    public override void OnTriggerUp()
    {

    }
}
