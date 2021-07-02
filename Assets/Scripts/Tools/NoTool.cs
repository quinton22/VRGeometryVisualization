using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoTool : ToolType
{
    public override Tool Name {
        get { return Tool.None; }
    }

    protected override void OnAwake() {
        toolTypeList.Add(this);
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
