using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawableLine : DrawableShape
{
    [SerializeField]
    private float m_LineThickness; // TODO

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
            m_Shape.transform.position = (drawingStartPositionNonNull + currentPosition) / 2;
            m_Shape.transform.localScale = new Vector3(
                m_Shape.transform.localScale.x, 
                (drawingStartPositionNonNull - currentPosition).magnitude / 2,
                m_Shape.transform.localScale.z);
            m_Shape.transform.localRotation = Quaternion.LookRotation((currentPosition - drawingStartPositionNonNull).normalized);
            m_Shape.transform.Rotate(90, 0, 0);
        }
        InvokeListener(ListenerType.PostDraw);
    }

    public override void StopDrawing()
    {
        InvokeListener(ListenerType.PreFinish);

        float sd = m_SubdivisionScale * 2;
        // round to nearest (sub)unit
        Vector3 scaleL = m_Shape.transform.localScale;
        scaleL.y *= sd;
        if (scaleL.y < 1) {
            scaleL.y = 1;
        }
        else
        {
            scaleL.y = Mathf.Round(scaleL.y);
        }
        scaleL.y /= sd;

        float deltaY = scaleL.y - m_Shape.transform.localScale.y;

        m_Shape.transform.localScale = scaleL;

        // move position to adjust for change of size
        Vector3 posL = m_Shape.transform.position;
        posL += m_Shape.transform.up * deltaY;
        m_Shape.transform.position = posL;

        base.StopDrawing();

        InvokeListener(ListenerType.PostFinish);
    }
}

