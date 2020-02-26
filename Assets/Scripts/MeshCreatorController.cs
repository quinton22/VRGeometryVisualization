using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCreatorController : MonoBehaviour
{
    private GameObject m_SubMeshGameObject;
    private Mesh m_Mesh;
    public GameObject m_Line;
    private Vector3 m_PositionOffset;
    private List<GameObject> m_MeshLines = new List<GameObject>();
    public List<Vector3> m_Vertices = new List<Vector3>();
    public List<int> m_Triangles = new List<int>();

    void Start()
    {
        m_SubMeshGameObject = transform.Find("SubMesh").gameObject;

        m_Mesh = new Mesh();
        m_SubMeshGameObject.GetComponent<MeshFilter>().mesh = m_Mesh;

        m_Line = transform.Find("Line").gameObject;
    }

    public GameObject GetLine(int index)
    {
        return m_MeshLines[index];
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

        UpdateTriangles();
        

        m_Mesh.vertices = m_Vertices.ToArray();
        m_Mesh.triangles = m_Triangles.ToArray();

        m_Mesh.RecalculateNormals();

        GameObject subMesh2 = Instantiate(m_SubMeshGameObject, m_SubMeshGameObject.transform.position, m_SubMeshGameObject.transform.localRotation, transform);
        Mesh mesh2 = subMesh2.GetComponent<MeshFilter>().mesh;

        // flip triangles
        List<int> triangles2 = new List<int>(mesh2.triangles);
        triangles2.Reverse();
        mesh2.triangles = triangles2.ToArray();

        mesh2.RecalculateNormals();

    }

    public void DeleteLines()
    {
        m_MeshLines.ForEach((GameObject line) => {
            Destroy(line);
        });
        m_MeshLines.Clear();
    }

    private void UpdateTriangles()
    {
        for (int i = 0; i < m_Vertices.Count; ++i)
        {
            if (i < 3)
                m_Triangles.Add(i);
            else
                m_Triangles.AddRange(new int[]{i-1, i, 0});
        }
    }
}
