using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteTool : ToolType<DrawableShape>
{
    public override Tool Name {
        get { return Tool.Delete; }
    }

    public override void OnTriggerDown()
    {
        // TODO: not this simple
        GameObject obj = m_PointerController.collidingObject;

        if (obj != null)
        {
            Destroy(obj);
        }
    }
}
