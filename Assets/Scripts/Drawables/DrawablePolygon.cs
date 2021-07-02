using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DrawablePolygon : DrawableShape
{
    private List<Vector3> m_Vertices = new List<Vector3>();
    private List<Vector3> verticesToCreateMesh = new List<Vector3>();
    private List<int> trianglesToCreateMesh = new List<int>();
    private Mesh m_Mesh;
    private MeshCollider m_MeshCollider;
    private GameObject m_PolygonToClone;
    private DrawableLine m_DrawableLine;
    private Vector3 offset;
    private bool isDrawingLine
    {
        get { return m_Vertices.Count < 2; }
    }
    private Vector3 norm
    {
        get { return Vector3.Cross(m_Vertices[1] - m_Vertices[0], m_Vertices[2] - m_Vertices[0]).normalized; }
    }
    [Tooltip("Fraction of the distance from 1st to 2nd point that user must click in to finish shape")]
    public float distFromPointToStopDrawing = 0.2f;

    // TODO: !!!!!!
    // to make a texture that can apply to the mesh, just need to update the UVs
    // to do this, take a bounding rect of the mesh
    //  
    //       (1,0)___________(1,1)
    //           | /      \  |
    //           |/        \ |
    //           |\         \|
    //           | \         |
    //           |__\_______/|
    //         (0,0)        (1, 0)
    // and at each vertex, apply the coordinates accordingly

    protected override void RunOnAwake()
    {
        base.RunOnAwake();
        m_PolygonToClone = m_ShapeToClone;
        m_DrawableLine = GetComponent<DrawableLine>();
        


        // // TESTING intersecting & colinear
        // Vector3 a1 = Vector3.zero;
        // Vector3 a2 = new Vector3(1, 1, 0);
        // Vector3 b1 = new Vector3(0.5f, 0.5f, 0);
        // Vector3 b2 = new Vector3(1.5f, 1.5f, 0);
        // print($"T1:: isColinear:: (true) :: {isColinear(a1, a2, b1, b2)}");
        // print($"T1:: isIntersecting:: (true) :: {isIntersecting(a1, a2, b1, b2)}");
        // b2 = new Vector3(0.75f, 0.75f, 0);
        // print($"T2:: isColinear:: (true) :: {isColinear(a1, a2, b1, b2)}");
        // print($"T2:: isIntersecting:: (true) :: {isIntersecting(a1, a2, b1, b2)}");
        // b2 = new Vector3(1, 1, 0);
        // b1 = Vector3.zero;
        // print($"T3:: isColinear:: (true) :: {isColinear(a1, a2, b1, b2)}");
        // print($"T3:: isIntersecting:: (true) :: {isIntersecting(a1, a2, b1, b2)}");
        // b2 = Vector3.one;
        // b1 = new Vector3(0.5f, 0.5f, 0);
        // print($"T4:: isColinear:: (false) :: {isColinear(a1, a2, b1, b2)}");
        // print($"T4:: isIntersecting:: (true) :: {isIntersecting(a1, a2, b1, b2)}");
        // b1 = new Vector3(0.5f, 1, 1);
        // print($"T5:: isColinear:: (false) :: {isColinear(a1, a2, b1, b2)}");
        // print($"T5:: isIntersecting:: (false) :: {isIntersecting(a1, a2, b1, b2)}");
        // b1 = new Vector3(0, 1, 0);
        // b2 = new Vector3(1, 0, 0);
        // print($"T6:: isColinear:: (false) :: {isColinear(a1, a2, b1, b2)}");
        // print($"T6:: isIntersecting:: (true) :: {isIntersecting(a1, a2, b1, b2)}");
    }

    public void AddPoint(Vector3 currentPosition)
    {
        if (m_Vertices.Count == 0)
        {
            offset = currentPosition;
        }
        if (m_Vertices.Count == 1)
        {
            StopDrawingLine();
            // TODO: is drawingStartPositionCorrect?
            
            m_Vertices.Add(currentPosition - offset);
        }

        if (ShouldStopDrawing(currentPosition - offset))
        {
            m_Vertices.RemoveAt(m_Vertices.Count - 1); // this was our current position, no longer needed
            StopDrawing(false); // TODO: ?? snapToGrid
        }
        else
        {
            m_Vertices.Add(currentPosition - offset);
        }
    }

    private void StopDrawingLine()
    {
        m_DrawableLine.StopDrawing(true); // TODO: figure out how to adjust snap to grid here
        m_DrawableLine.DeleteLine();
    }

    private bool ShouldStopDrawing(Vector3 position)
    {
        return position.magnitude <= distFromPointToStopDrawing;
    }

    public override void StartDrawing(Vector3 startPosition)
    {
        InvokeListener(ListenerType.PreStart);
        m_DrawableLine.StartDrawing(startPosition);
        AddPoint(startPosition); // TODO this should be called by the polygon tool probably
        InvokeListener(ListenerType.PostStart);
    }

    public override void Drawing(Vector3 currentPosition)
    {
        InvokeListener(ListenerType.PreDraw);
        base.Drawing(currentPosition);
        if (isDrawingLine)
        {
            m_DrawableLine.Drawing(currentPosition);
        }
        else
        {
            m_Vertices[m_Vertices.Count - 1] = currentPosition - offset;
            UpdateMesh();
        }
        InvokeListener(ListenerType.PostDraw);
    }

    public override void StopDrawing(bool snapToGrid)
    {
        InvokeListener(ListenerType.PreFinish);
        base.StopDrawing(snapToGrid);


        InvokeListener(ListenerType.PostFinish);
    }

    private void CreateMesh()
    {
        m_Shape = UnityEngine.Object.Instantiate(m_PolygonToClone, drawingStartPositionNonNull, Quaternion.identity, m_ParentTransform);
        m_Mesh = new Mesh();
        m_Shape.GetComponent<MeshFilter>().sharedMesh = m_Mesh;

        m_MeshCollider = m_Shape.GetComponent<MeshCollider>();
        m_MeshCollider.sharedMesh = m_Mesh;
        m_Mesh.Clear();
        m_Mesh.MarkDynamic();
    }

    private void UpdateMesh()
    {
        SetVertexList();
        //TODO: do this on update or add point?
        SetTrianglesList();
        m_Mesh.Optimize();
        m_Mesh.RecalculateNormals();
        m_Mesh.RecalculateBounds();

        m_MeshCollider.sharedMesh = m_Mesh;
    }

    private void SetVertexList(float height = .001f)
    {
        verticesToCreateMesh.Clear();

        verticesToCreateMesh.AddRange(m_Vertices);
        verticesToCreateMesh.AddRange(m_Vertices.Select(vertex => vertex - norm * height));
        m_Mesh.vertices = verticesToCreateMesh.ToArray();
    }

    private void SetTrianglesList()
    {
        trianglesToCreateMesh.Clear();

        int numVerticesOnTopFace = verticesToCreateMesh.Count / 2;
        for (int i = 0; i < numVerticesOnTopFace; ++i)
        {
            if (i < 3)
            {
                trianglesToCreateMesh.Add(i);
            }
            else
            {
                // add 0, i-1, i
                // check if (i-1)->i intersects any other set of points
                // TODO: isn't checking (n-2)->(n-1) & (n-1)->n intersect ... maybe create (n-2)->(n-1) * 1.01 + (n-1)
                // if (!doesIntersectVerticesInList(verticesToCreateMesh[i-1], verticesToCreateMesh[i], verticesToCreateMesh.GetRange(0, numVerticesOnTopFace)) &&
                //     !doesIntersectVerticesInList())
                // {

                // }
            }
            /*
                0, 1, 2,
                0, 2, 3

            */
        }

        m_Mesh.triangles = trianglesToCreateMesh.ToArray();
    }

    // v1, v2, v3, v4 must be presented clockwise
    private void AddQuadToTrianglesList(int v1, int v2, int v3, int v4)
    {
        trianglesToCreateMesh.AddRange(new int[] { v1, v2, v3, v1, v3, v4 });
    }

    private bool doesIntersectVerticesInList(Vector3 a, Vector3 b, List<Vector3> vertices)
    {
        for (int i = 0; i < vertices.Count - 1; ++i)
        {
            if (isIntersecting(a, b, vertices[i], vertices[i-1])) return true;
        }
        return false;
    }

    private bool isColinear(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2)
    {
        Vector3 A = (a1 - a2);
        Vector3 B = (b1 - b2);
        Vector3 C = (a1 - b1);

        if (Vector3.Cross(A, B) == Vector3.zero && Vector3.Cross(A, C) == Vector3.zero)
        {
            return true;
        }

        return false;
    }

    private bool isIntersecting(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2)
    {
        if (isColinear(a1, a2, b1, b2))
        {
            Vector3 A = a2 - a1;
            Vector3 B = b1 - a1;
            Vector3 C = b2 - a1;

            float Bcos = Vector3.Dot(A, B) / A.magnitude;
            float Ccos = Vector3.Dot(A, C) / A.magnitude;

            if ((Bcos >= 0 && Bcos <= A.magnitude) || (Ccos >= 0 && Ccos <= A.magnitude))
            {
                return true;
            }
        }
        else
        {
            Plane plane = new Plane(a1, a2, b1);
            if (plane.GetDistanceToPoint(b2) < 0.0001)
            {
                // all on same plane
                Vector3 A = a2 - a1;
                Vector3 B = b1 - a1;
                Vector3 C = b2 - a1;

                float angle1 = Vector3.Angle(A, B);
                float angle2 = Vector3.Angle(A, C);
                float angle3 = Vector3.Angle(B, C);

                if (angle1 + angle2 == angle3)
                {
                    return true;
                }
            }
        }


        return false;
    }
}
