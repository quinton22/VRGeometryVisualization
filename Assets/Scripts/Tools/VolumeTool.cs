using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeTool : ToolType<DrawableVolume>
{
    public override Tool Name {
        get { return Tool.Volume; }
    }

    void Awake() {
        m_DrawableShape = GetComponent<DrawableVolume>();
    }

    // TODO: these should probably be removed at some point
    private Vector3 initialPosition;
    private MeshCreatorController m_MeshCreatorController;

    public override void OnTriggerDown()
    {
        if (m_PointerController.collidingObject != null)
        {
            m_PointerController.IncreaseSizeOfPointer();

            if (m_PointerController.collidingObject.GetComponent<ShapeType>().m_ShapeType == ShapeTypeEnum.Area)
            {
                GameObject area = m_PointerController.collidingObject.transform.parent.gameObject;
                // TODO: change back
                m_DrawableShape.m_AreaLocalRotation = area.transform.localRotation;
                m_DrawableShape.m_AreaLocalScale = area.transform.localScale;
                m_DrawableShape.m_AreaForwardDir = area.transform.forward;
                m_DrawableShape.StartDrawing(area.transform.position);
            }
            else if (m_PointerController.collidingObject.GetComponent<ShapeType>().m_ShapeType == ShapeTypeEnum.Polygon)
            {
                // start extruding mesh
                GameObject polygon = m_PointerController.collidingObject;
               // drawing = Tool.Volume;
                initialPosition = polygon.transform.position;
                
                m_MeshCreatorController = polygon.GetComponent<MeshCreatorController>();
            }

            OnTriggerHold();
        } 
    }

    public override void OnTriggerHold()
    {
        base.OnTriggerHold();
    }

    public override void OnTriggerUp()
    {
        base.OnTriggerUp();
    }
}
