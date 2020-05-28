using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Controls the grid and scale shown for each mesh/line/volume
public class MeshGridController : MonoBehaviour
{
    private GameObject m_Parent;
    private RectTransform m_RectTransform;
    private Vector3 m_BaseScale;
    private Vector2 m_BaseWidthHeight;
    private GameObject m_OriginalImage;
    private GameObject m_CrossAxisOriginalImage;
    private List<RectTransform> MainAxisTickMarks = new List<RectTransform>();
    private List<RectTransform> CrossAxisTickMarks = new List<RectTransform>();
    private float m_ScaleAmount;
    private float scaleMultiplier = 1;
    private MeshCreatorController m_MeshCreator;
    [SerializeField]
    [Tooltip("Only need on children of Mesh object")]
    private GameObject m_Pointer;
    private PointerController m_PointerController;

    void Start()
    {
        m_Parent = transform.parent.gameObject;
        m_RectTransform = gameObject.GetComponent<RectTransform>();
        m_BaseScale = m_RectTransform.localScale;
        m_BaseWidthHeight = m_RectTransform.sizeDelta;

        m_MeshCreator = m_Parent.GetComponent<MeshCreatorController>();
        m_PointerController = m_Pointer.GetComponent<PointerController>();
        transform.localScale = Vector3.zero;
        m_OriginalImage = transform.Find("Image (Horz)").gameObject;
        m_CrossAxisOriginalImage = transform.Find("Image (Vert)").gameObject;

        UpdateGridScale();
        GlobalGridScale.AddScaleListener(UpdateGridScale);
    }

    // Update is called once per frame
    void Update()
    {
        MeshUpdate();
    }

    private void UpdateGridScale()
    {
        m_ScaleAmount = 1000f / (float)GlobalGridScale.Instance.GridScale * scaleMultiplier;
    }

    void MeshUpdate()
    {
        if (m_MeshCreator.Finished || m_MeshCreator.m_Vertices.Count < 2) return;

        // want mesh to expand to greatest point on plane in x and y direction
        // including the current pointerc

        Vector3 origin = m_MeshCreator.m_Vertices[0];
        float yMax = 0;
        float xMax = 0;
        float yMin = 0;
        float xMin = 0;

        Vector3 yDir = m_MeshCreator.m_Vertices[1].normalized;
        Vector3 xDir = Vector3.zero;

        Vector3 normal = Vector3.zero;

        if (m_MeshCreator.m_Vertices.Count == 2)
        {
            // need to use the current position of the pointer for the 3rd point for the plane
            Vector3 b = m_MeshCreator.m_Vertices[1];
            Vector3 c = m_Pointer.transform.position - m_MeshCreator.m_PositionOffset;

            normal = Vector3.Cross(b, c);
            Vector3 yProj = Vector3.Project(c, b.normalized);

            if ((yProj.normalized + b.normalized).magnitude > 1)
            {
                if (b.magnitude > yProj.magnitude)
                {
                    yMax = b.magnitude;
                }
                else
                {
                    yMax = yProj.magnitude;
                }
            }
            else
            {
                yMax = b.magnitude;
            }


            if ((yProj.normalized + b.normalized).magnitude < 1 && -yProj.magnitude < yMin)
            {
                yMin = -yProj.magnitude;
            }

            xDir = Vector3.Cross(Vector3.Cross(b, c), b).normalized;
            xMax = Vector3.Project(c, xDir).magnitude;

            // Debug.DrawLine(a + m_MeshCreator.m_PositionOffset, b + m_MeshCreator.m_PositionOffset, Color.red, Time.deltaTime);
            // Debug.DrawLine(a + m_MeshCreator.m_PositionOffset, c + m_MeshCreator.m_PositionOffset, Color.blue, Time.deltaTime);
        }
        else if (m_MeshCreator.m_Vertices.Count >= 3)
        {
            normal = m_MeshCreator.GetNorm();

            xDir = Vector3.Cross(Vector3.Cross(yDir, m_MeshCreator.m_Vertices[2].normalized), yDir).normalized;


            foreach (Vector3 v in m_MeshCreator.m_Vertices)
            {
                Vector3 yProj = Vector3.Project(v, yDir);
                if ((yProj.normalized + yDir.normalized).magnitude > 1 && yProj.magnitude > yMax)
                {
                    yMax = yProj.magnitude;
                }
                if ((yProj.normalized + yDir.normalized).magnitude < 1 && -yProj.magnitude < yMin) // yProj and yDir are in the opposite direction
                {
                    yMin = -yProj.magnitude;
                }

                Vector3 xProj = Vector3.Project(v, xDir);
                if ((xProj.normalized + xDir.normalized).magnitude > 1 && xProj.magnitude > xMax)
                {
                    xMax = xProj.magnitude;
                }
                if ((xProj.normalized + xDir.normalized).magnitude < 1 && -xProj.magnitude < xMin) // xProj and xDir are in the opposite direction
                {
                    xMin = -xProj.magnitude;
                }
            }

            Vector3 v2 = m_Pointer.transform.position - m_MeshCreator.m_PositionOffset;

            Vector3 yProj2 = Vector3.Project(v2, yDir);
            if ((yProj2.normalized + yDir.normalized).magnitude > 1 && yProj2.magnitude > yMax)
            {
                yMax = yProj2.magnitude;
            }
            if ((yProj2.normalized + yDir.normalized).magnitude < 1 && -yProj2.magnitude < yMin) // yProj and yDir are in the opposite direction
            {
                yMin = -yProj2.magnitude;
            }

            Vector3 xProj2 = Vector3.Project(v2, xDir);
            if ((xProj2.normalized + yDir.normalized).magnitude > 1 && xProj2.magnitude > xMax)
            {
                xMax = xProj2.magnitude;
            }
            if ((xProj2.normalized + xDir.normalized).magnitude < 1 && -xProj2.magnitude < xMin) // xProj and xDir are in the opposite direction
            {
                xMin = -xProj2.magnitude;
            }


        }

        if (m_MeshCreator.m_Vertices.Count >= 2 && transform.localScale == Vector3.zero && normal != Vector3.zero)
        {
            transform.localScale = m_BaseScale;
        }
        else if (m_MeshCreator.m_Vertices.Count >= 2 && transform.localScale == Vector3.zero && normal == Vector3.zero)
        {
            return;
        }


        m_RectTransform.rotation = Quaternion.LookRotation(-normal, yDir);
        m_RectTransform.sizeDelta = new Vector2(m_BaseWidthHeight.x * (xMax - xMin), m_BaseWidthHeight.y * (yMax - yMin));
        m_RectTransform.localPosition = (gameObject.name.ToLower().Contains("flipped") ? -5 : 1) * .001f * normal + xMin * xDir + yMin * yDir;

        if (CrossAxisTickMarks.Count == 0)
        {
            CrossAxisTickMarks.Add(m_CrossAxisOriginalImage.GetComponent<RectTransform>());
        }

        if (MainAxisTickMarks.Count == 0)
        {
            MainAxisTickMarks.Add(m_OriginalImage.GetComponent<RectTransform>());
        }

        float numMarkers = Mathf.Ceil(m_RectTransform.sizeDelta.x / m_ScaleAmount);
        int count = MainAxisTickMarks.Count;

        if (count < numMarkers)
        {
            // add more ticks
            for (int i = count; i < numMarkers; ++i)
            {
                GameObject newImage = Instantiate(m_OriginalImage, Vector3.zero, Quaternion.identity);
                newImage.transform.SetParent(transform, false);

                MainAxisTickMarks.Add(newImage.GetComponent<RectTransform>());
            }
        }
        else if (count > numMarkers && count > 1)
        {
            // delete ticks
            for (int i = count - 1; i >= numMarkers; --i)
            {
                GameObject temp = MainAxisTickMarks[i].gameObject;
                MainAxisTickMarks.RemoveAt(i);
                Destroy(temp);
            }
        }

        int numBelow = (int)Mathf.Floor((-xMin * m_BaseWidthHeight.x) / m_ScaleAmount);
        int numAbove = 0;

        foreach (RectTransform tick in MainAxisTickMarks)
        {
            float x = 0;
            if (numBelow > 0)
            {
                x = -xMin * m_BaseWidthHeight.x - numBelow-- * m_ScaleAmount;
            }
            else
            {
                x = -xMin * m_BaseWidthHeight.x + numAbove++ * m_ScaleAmount;
            }

            tick.anchoredPosition = new Vector2(x, 0);
        }

        numMarkers = Mathf.Ceil(m_RectTransform.sizeDelta.y / m_ScaleAmount);
        count = CrossAxisTickMarks.Count;

        if (count < numMarkers)
        {
            // add more ticks
            for (int i = count; i < numMarkers; ++i)
            {
                GameObject newImage = Instantiate(m_CrossAxisOriginalImage, Vector3.zero, Quaternion.identity);
                newImage.transform.SetParent(transform, false);

                CrossAxisTickMarks.Add(newImage.GetComponent<RectTransform>());
            }
        }
        else if (count > numMarkers && count > 1)
        {
            // delete ticks
            for (int i = count - 1; i >= numMarkers; --i)
            {
                GameObject temp = CrossAxisTickMarks[i].gameObject;
                CrossAxisTickMarks.RemoveAt(i);
                Destroy(temp);
            }
        }

        numBelow = (int)Mathf.Floor((-yMin * m_BaseWidthHeight.y) / m_ScaleAmount);
        numAbove = 0;

        foreach (RectTransform tick in CrossAxisTickMarks)
        {
            float y = 0;
            if (numBelow > 0)
            {
                y = -yMin * m_BaseWidthHeight.y - numBelow-- * m_ScaleAmount;
            }
            else
            {
                y = -yMin * m_BaseWidthHeight.y + numAbove++ * m_ScaleAmount;
            }

            tick.anchoredPosition = new Vector2(0, y);
        }

        // adjust length of each of the ticks accordingly
        foreach (RectTransform tick in MainAxisTickMarks)
        {
            Vector3 tickSize = tick.sizeDelta;
            tickSize.y = m_RectTransform.sizeDelta.y;
            tick.sizeDelta = tickSize;
        }

        foreach (RectTransform tick in CrossAxisTickMarks)
        {
            Vector3 tickSize = tick.sizeDelta;
            tickSize.x = m_RectTransform.sizeDelta.x;
            tick.sizeDelta = tickSize;
        }
    }
}
