using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTool : ToolType
{
    public override Tool Name {
        get { return Tool.Area; }
    }

    private GameObject line;

    protected override void OnAwake() {
        m_DrawableShape = GetComponent<DrawableArea>();
        if (isToolEnabled)
            toolTypeList.Add(this);
    }

    public override void OnTriggerDown()
    {
        base.OnTriggerDown();
        if (m_PointerController.collidingObject != null && m_PointerController.collidingObject.GetComponent<ShapeType>().m_ShapeType == ShapeTypeEnum.Line)
        {   
            m_PointerController.IncreaseSizeOfPointer();

            line = m_PointerController.collidingObject;

            // TODO: change back
            (m_DrawableShape as DrawableArea).m_UpVec = line.transform.up;
            (m_DrawableShape as DrawableArea).m_YScale = line.transform.localScale.y;
            m_DrawableShape.StartDrawing(line.transform.position);
            
            // Dragged(); // TODO: do we need? if so add to post start
        }
    }

    public override void OnTriggerHold()
    {
        if (!restrictedPlane.HasValue)
        {
            base.OnTriggerHold();
        }
        else
        {
            Vector3 closestPoint = restrictedRect != null
                ? restrictedRect.ClosestPointOnRect(m_Pointer.transform.position)
                : restrictedPlane.Value.ClosestPointOnPlane(m_Pointer.transform.position);
            if (m_DrawableShape)
            {
                m_DrawableShape.Drawing(closestPoint);
            }
        }
    }

    public override void OnTriggerUp()
    {
        base.OnTriggerUp();
        Destroy(line);
    }
}
