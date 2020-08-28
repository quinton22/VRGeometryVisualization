using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTool : ToolType<DrawableArea>
{
    public override Tool Name {
        get { return Tool.Area; }
    }

    protected override void OnAwake() {
        m_DrawableShape = GetComponent<DrawableArea>();
    }

    public override void OnTriggerDown()
    {
        if (m_PointerController.collidingObject != null && m_PointerController.collidingObject.GetComponent<ShapeType>().m_ShapeType == ShapeTypeEnum.Line)
        {   
            m_PointerController.IncreaseSizeOfPointer();

            GameObject line = m_PointerController.collidingObject;

            // TODO: change back
            (m_DrawableShape as DrawableArea).m_UpVec = line.transform.up;
            (m_DrawableShape as DrawableArea).m_YScale = line.transform.localScale.y;
            m_DrawableShape.StartDrawing(line.transform.position);
            
            // Dragged(); // TODO: do we need? if so add to post start
        }
    }
}
