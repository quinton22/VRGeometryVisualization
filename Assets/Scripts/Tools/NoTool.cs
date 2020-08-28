using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoTool : ToolType<DrawableShape>
{
    public override Tool Name {
        get { return Tool.None; }
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
