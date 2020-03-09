using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshCreatorController : MonoBehaviour
{
    private Mesh m_Mesh;
    private MeshCollider m_MeshCollider;
    public GameObject m_Line;
    public Vector3 m_PositionOffset;
    private Vector3[] m_AreaVertices;
    private List<GameObject> m_MeshLines = new List<GameObject>();
    public List<Vector3> m_Vertices = new List<Vector3>();
    public List<int> m_Triangles = new List<int>();

    void Start()
    {
        // gameObject = transform.Find("SubMesh").gameObject;

        m_Mesh = new Mesh();
        GetComponent<MeshFilter>().sharedMesh = m_Mesh;

        m_MeshCollider = GetComponent<MeshCollider>();
        m_MeshCollider.sharedMesh = m_Mesh;


        m_Line = transform.Find("Line").gameObject;
    }

    public GameObject GetLine(int index)
    {
        return m_MeshLines[index];
    }

    public List<GameObject> GetLines()
    {
        return m_MeshLines;
    }

    public GameObject AddLine(Vector3 position)
    {
        GameObject line = Instantiate(m_Line, position, Quaternion.identity, transform);
        m_MeshLines.Add(line);

        if (m_MeshLines.Count == 1)
        {
            m_PositionOffset = position;
        }

        m_Vertices.Add(position - m_PositionOffset);

        if (m_MeshLines.Count > 1)
        {
            Destroy(line.transform.Find("Sphere").gameObject);
        }

        return line;
    }

    public void CreateMesh()
    {
        if (m_Vertices.Count < 3)
            return;

        m_AreaVertices = m_Vertices.ToArray();

        m_Mesh.Clear();
        RealUpdateMesh();
    }

    public void FinishMesh()
    {
        Destroy(m_Line);
    }

    private void RealUpdateMesh(float length = .001f)
    {
        //m_Mesh.Clear();

        m_Mesh.MarkDynamic();

        UpdateVertices(length);
        m_Mesh.vertices = m_Vertices.ToArray();

        UpdateTriangles();
        m_Mesh.triangles = m_Triangles.ToArray();

        m_Mesh.Optimize();
        m_Mesh.RecalculateNormals();
        m_Mesh.RecalculateBounds();

        m_MeshCollider.sharedMesh = m_Mesh;
    }

    public void UpdateMesh(Vector3 dir)
    {
        float m = (dir.normalized - GetNorm()).magnitude;

        float update = dir.magnitude;

        if (m > .00001) // same direction as norm
        {
            for (int i = 0; i < m_AreaVertices.Length; ++i)
            {
                m_Vertices[i] = m_AreaVertices[i];
            }
        }
        else
        {
            for (int i = 0; i < m_AreaVertices.Length; ++i)
            {
                m_Vertices[i] = m_AreaVertices[i] + dir;
            }

            update = 0;
        }
        
        m_Vertices.RemoveRange(m_AreaVertices.Length, 5*m_AreaVertices.Length);

        RealUpdateMesh(update);
    }

    public void DeleteLines()
    {
        m_MeshLines.ForEach((GameObject line) => {
            Destroy(line);
        });
        m_MeshLines.Clear();
    }

    public Vector3 GetNorm()
    {
        return Vector3.Cross(m_Vertices[1] - m_Vertices[0], m_Vertices[2] - m_Vertices[0]).normalized;
    }

    private void UpdateVertices(float length = .001f)
    {
        // m_Vertices always contains the initial area vertices
        int numVertices = m_AreaVertices.Length;
        Vector3 norm = GetNorm();
        for (int i = 0; i < numVertices; ++i)
        {
            m_Vertices.Add(m_AreaVertices[i] - norm*length);
        }

        for (int i = 1; i < numVertices + 1; ++i)
        {
            m_Vertices.AddRange(new Vector3[]{
                m_Vertices[i % numVertices],
                m_Vertices[i-1],
                m_Vertices[i-1+numVertices],
                m_Vertices[(i % numVertices) + numVertices]
            });
        }
    }

    private void UpdateTriangles()
    {
        m_Triangles.Clear();
        int count = m_AreaVertices.Length;
        for (int i = 0; i < count; ++i)
        {
            if (i < 3)
                m_Triangles.Add(i);
            else
                m_Triangles.AddRange(new int[]{i-1, i, 0});
        }

        for (int i = 2*count-1; i >= count; --i)
        {
            if (i < count + 3)
                m_Triangles.Add(i);
            else
                m_Triangles.AddRange(new int[]{i, i-1, count});
        }

        for (int i = 2*count; i < m_Vertices.Count; i+=4)
        {
            m_Triangles.AddRange(new int[]{ i, i+1, i+2, i, i+2, i+3 });
        }
    }

    public void CreateExtrudedMesh()
    {
       
    }
}
