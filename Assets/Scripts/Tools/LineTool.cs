using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTool : ToolType
{
    public override Tool Name {
        get { return Tool.Line; }
    }

    protected override void OnAwake() {
        m_DrawableShape = GetComponent<DrawableLine>();
        if (isToolEnabled)
            toolTypeList.Add(this);
    }

    public override void OnTriggerDown()
    {
        base.OnTriggerDown();

        if (restrictedPlane.HasValue)
        {

            // Only allow for drawing if intersecting with the restrictedPlane
            if (restrictedPlane.Value.GetDistanceToPoint(m_Pointer.transform.position) > m_Pointer.transform.localScale.x)
            {
                return;
            }
            if (restrictedRect != null)
            {
                if (!restrictedRect.Contains(m_Pointer.transform.position, m_Pointer.transform.localScale.x))
                {
                    return;
                }
            }
        }

        m_PointerController.IncreaseSizeOfPointer();


        Vector3 closestPoint = restrictedPlane.HasValue ? restrictedPlane.Value.ClosestPointOnPlane(m_Pointer.transform.position) : m_Pointer.transform.position;
        // TODO:
        m_DrawableShape.StartDrawing(closestPoint);
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
    }
}
