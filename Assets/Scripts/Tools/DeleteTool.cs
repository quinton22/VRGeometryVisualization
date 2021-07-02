using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteTool : ToolType
{
    public override Tool Name {
        get { return Tool.Delete; }
    }

    protected override void OnAwake() {
        if (isToolEnabled)
            toolTypeList.Add(this);
    }

    public override void OnTriggerDown()
    {
        base.OnTriggerDown();
        // TODO: not this simple
        GameObject obj = m_PointerController.collidingObject;

        if (obj != null)
        {
            Destroy(obj);
        }
    }

    public override void OnTriggerHold() {}
    public override void OnTriggerUp() {}
}
