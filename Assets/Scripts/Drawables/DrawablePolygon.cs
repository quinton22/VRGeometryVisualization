using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawablePolygon : DrawableShape
{
    private List<Vector3> m_Vertices = new List<Vector3>();
    private List<int> m_Triangles = new List<int>();

    public override void StartDrawing(Vector3 startPosition)
    {  
        InvokeListener(ListenerType.PreStart);
        base.StartDrawing(startPosition);
        InvokeListener(ListenerType.PostStart);
    }

    public override void Drawing(Vector3 currentPosition)
    {
        // InvokeListener(ListenerType.PreDraw);
        // base.Drawing(currentPosition);
        // if (m_Shape != null)
        // {
            




        //     // set rotation of area so it is locked in the axis that is represented by the line
        //     Vector3 line_x = Vector3.Cross((currentPosition - drawingStartPositionNonNull).normalized, m_UpVec);            
        //     m_Shape.transform.localRotation = Quaternion.LookRotation(line_x, m_UpVec).normalized;

        //     m_Shape.transform.localScale = new Vector3(
        //         Vector3.Project((currentPosition - drawingStartPositionNonNull), m_Shape.transform.right).magnitude,
        //         m_YScale * 2,
        //         1
        //     );

        //     m_Shape.transform.position = drawingStartPositionNonNull + m_Shape.transform.right * m_Shape.transform.localScale.x / 2;
       
        // }
        // InvokeListener(ListenerType.PostDraw);
    }

    public override void StopDrawing(bool snapToGrid)
    {
        InvokeListener(ListenerType.PreFinish);
        base.StopDrawing(snapToGrid);

        Vector3 scale = m_Shape.transform.localScale;
        scale.x = base.SnapToGrid(scale.x, subdivisionScale: m_SubdivisionScale);

        float deltaX = scale.x - m_Shape.transform.localScale.x;

        m_Shape.transform.localScale = scale;

        // move position
        Vector3 posA = m_Shape.transform.position;
        posA += m_Shape.transform.right * deltaX / 2;
        m_Shape.transform.position = posA;

        InvokeListener(ListenerType.PostFinish);
    }
}
