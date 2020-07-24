using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawableSphere : DrawableShape
{
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
            float diameter = (drawingStartPositionNonNull - currentPosition).magnitude * 2;
            m_Shape.transform.localScale = new Vector3(
                diameter,
                diameter,
                diameter
            );
        }
        InvokeListener(ListenerType.PostDraw);
    }

    public override void StopDrawing(bool snapToGrid)
    {
        InvokeListener(ListenerType.PreFinish);
        base.StopDrawing(snapToGrid);

        Vector3 scale = m_Shape.transform.localScale;
        scale.x = base.SnapToGrid(scale.x, subdivisionScale: m_SubdivisionScale / 2);
        float deltaX = scale.x - m_Shape.transform.localScale.x;

        m_Shape.transform.localScale = scale;

        InvokeListener(ListenerType.PostFinish);
    }
}

