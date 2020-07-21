using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawableVolume : DrawableShape
{
    private Vector3 m_VolumeForward;
    [System.NonSerialized] public GameObject m_LastCreatedShape;
    [System.NonSerialized] public Quaternion m_AreaLocalRotation;
    [System.NonSerialized] public Vector3 m_AreaLocalScale;
    [System.NonSerialized] public Vector3 m_AreaForwardDir;

    public override void StartDrawing(Vector3 startPosition)
    {  
        InvokeListener(ListenerType.PreStart);
        base.StartDrawing(startPosition);
        InvokeListener(ListenerType.PostStart);
    }

    public override void Drawing(Vector3 currentPosition)
    {
        InvokeListener(ListenerType.PreDraw);
        base.Drawing(currentPosition);
        if (m_Shape != null)
        {
            m_Shape.transform.localRotation = m_AreaLocalRotation;
            m_Shape.transform.localScale = m_AreaLocalScale;

            m_VolumeForward = Vector3.Project((currentPosition - drawingStartPositionNonNull), m_Shape.transform.forward);

            m_Shape.transform.localScale = new Vector3(
                m_Shape.transform.localScale.x,
                m_Shape.transform.localScale.y,
                m_VolumeForward.magnitude
            );

            m_Shape.transform.position = drawingStartPositionNonNull + m_VolumeForward.normalized * m_Shape.transform.localScale.z / 2;

            //m_Shape.GetComponent<VolumeForwardController>().dir = Vector3.Project((currentPosition - drawingStartPositionNonNull), -m_AreaForwardDir);
       
        }
        InvokeListener(ListenerType.PostDraw);
    }

    public override void StopDrawing()
    {
        InvokeListener(ListenerType.PreFinish);

        Vector3 scaleV = m_Shape.transform.localScale;
        scaleV.z *= m_SubdivisionScale;
        if (scaleV.z < 1)
        {
            scaleV.z = 1;
        }
        else
        {
            scaleV.z = Mathf.Round(scaleV.z);
        }

        scaleV.z /= m_SubdivisionScale;

        float deltaZ = scaleV.z - m_Shape.transform.localScale.z;

        m_Shape.transform.localScale = scaleV;

        // move position
        Vector3 posV = m_Shape.transform.position;
        posV += m_VolumeForward.normalized * deltaZ / 2;
        m_Shape.transform.position = posV;


        m_LastCreatedShape = m_Shape;
        base.StopDrawing();

        InvokeListener(ListenerType.PostFinish);
    }
}
