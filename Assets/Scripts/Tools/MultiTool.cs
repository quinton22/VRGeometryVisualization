using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiTool : ToolType<DrawableShape>
{
    public override Tool Name {
        get { return Tool.Multi; }
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
