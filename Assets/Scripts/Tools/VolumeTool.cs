using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeTool : ToolType
{
    public override Tool Name {
        get { return Tool.Volume; }
    }

    private GameObject polygon;

    protected override void OnAwake() {
        m_DrawableShape = GetComponent<DrawableVolume>();
        if (isToolEnabled)
            toolTypeList.Add(this);
    }

    // TODO: these should probably be removed at some point
    private Vector3 initialPosition;
    private MeshCreatorController m_MeshCreatorController;

    public override void OnTriggerDown()
    {
        base.OnTriggerDown();
        if (m_PointerController.collidingObject != null)
        {
            m_PointerController.IncreaseSizeOfPointer();

            if (m_PointerController.collidingObject.GetComponent<ShapeType>().m_ShapeType == ShapeTypeEnum.Area)
            {
                polygon = m_PointerController.collidingObject.transform.parent.gameObject;
                // TODO: change back
                (m_DrawableShape as DrawableVolume).m_AreaLocalRotation = polygon.transform.localRotation;
                (m_DrawableShape as DrawableVolume).m_AreaLocalScale = polygon.transform.localScale;
                (m_DrawableShape as DrawableVolume).m_AreaForwardDir = polygon.transform.forward;
                m_DrawableShape.StartDrawing(polygon.transform.position);
            }
            else if (m_PointerController.collidingObject.GetComponent<ShapeType>().m_ShapeType == ShapeTypeEnum.Polygon)
            {
                // start extruding mesh
                polygon = m_PointerController.collidingObject;
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
        Destroy(polygon);
    }
}
