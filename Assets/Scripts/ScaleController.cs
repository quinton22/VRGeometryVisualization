using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// ScaleController
// Controls the grid and scale shown for each mesh/line/volume
public class ScaleController : MonoBehaviour
{
    private GameObject m_Parent;
    private RectTransform m_RectTransform;
    private Vector3 m_BaseScale;
    private Vector2 m_BaseWidthHeight;
    private GameObject m_OriginalImage;
    private GameObject m_CrossAxisOriginalImage;
    private Action<string> UpdateSpecific;
    private List<RectTransform> CrossAxisTickMarks = new List<RectTransform>();
    private string m_ScaleDim;
    private bool m_FollowCamera = false;
    private float m_ScaleAmount = 500; // TODO: hook this up with the scale division in inputcontroller
    private bool m_Volume = false;
    private VolumeForwardController m_VolumeForwardController;
    // Start is called before the first frame update
    void Start()
    {
        m_Parent = transform.parent.gameObject;
        m_RectTransform = gameObject.GetComponent<RectTransform>();
        m_BaseScale = m_RectTransform.localScale;
        m_BaseWidthHeight = m_RectTransform.sizeDelta;

        UpdateSpecific = NOP;

        if (m_Parent.name.Contains("Clone"))
        {
            if (m_Parent.name.Contains("Line"))
            {
                m_ScaleDim = "y";
                m_FollowCamera = true;
                m_ScaleAmount = 250;
                m_OriginalImage = transform.Find("Image").gameObject;
            }

            else if (m_Parent.name.Contains("Volume"))
            {
                m_VolumeForwardController = m_Parent.GetComponent<VolumeForwardController>();
                m_ScaleDim = "x";
                m_OriginalImage = transform.Find("Image (Horz)").gameObject; // TODO: rename
                m_CrossAxisOriginalImage = transform.Find("Image (Vert)").gameObject;
                m_Volume = true;
                AdjustCrossAxisTicks();
                SetInitial();

                Debug.Log($"{gameObject.name}");
            }
            else if (m_Parent.name.Contains("Mesh"))
            {
                m_ScaleDim = "";
                m_OriginalImage = transform.Find("Image (Horz)").gameObject;
                m_CrossAxisOriginalImage = transform.Find("Image (Vert)").gameObject;
                CreateGridForMesh();
            }
            UpdateSpecific = SpecificUpdate;
        }
        else if (m_Parent.transform.parent.gameObject.name.Contains("Clone") && m_Parent.transform.parent.gameObject.name.Contains("Area"))
        {
            m_ScaleDim = "x";
            
            m_Parent = m_Parent.transform.parent.gameObject;
            m_OriginalImage = transform.Find("Image (Horz)").gameObject; // TODO: rename
            m_CrossAxisOriginalImage = transform.Find("Image (Vert)").gameObject;
            UpdateSpecific = SpecificUpdate;
            AdjustCrossAxisTicks();
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSpecific(m_ScaleDim);
        if (m_Volume) {
            RotateForVolume();
        }
    }

    void SpecificUpdate(string scaleDim) {
        Vector3 scale = m_RectTransform.localScale;

        // TODO: update in beginning
        if (m_Volume && (gameObject.name.ToLower() == "top" || gameObject.name.ToLower() == "bottom"))
            return;

        if (scaleDim.Contains("x"))
            UpdateXScale(ref scale);
        if (scaleDim.Contains("y"))
            UpdateYScale(ref scale);
        
        m_RectTransform.localScale = scale;

        Vector3 size = m_RectTransform.sizeDelta;

        if (scaleDim.Contains("x"))
            UpdateXSize(ref size);
        if (scaleDim.Contains("y"))
            UpdateYSize(ref size);

        m_RectTransform.sizeDelta = size;

        if (CrossAxisTickMarks.Count != 0)
        {
            CrossAxisTickMarks.ForEach((RectTransform tick) => {
                Vector3 tickSize = tick.sizeDelta;
                tickSize.x = size.x;
                tick.sizeDelta = tickSize;
            });
        }

        float numMarkers = 0;
        if (scaleDim.Contains("y"))
            numMarkers = Mathf.Floor(size.y / m_ScaleAmount); // TODO: change 250 to something that is variable (will be different for line vs area)
        else if (scaleDim.Contains("x"))
            numMarkers = Mathf.Floor(size.x / m_ScaleAmount);

        float childCount = transform.childCount;
        float count = childCount - CrossAxisTickMarks.Count;

        if (count < numMarkers + 1)
        {
            Vector3 newPos = new Vector3(0, numMarkers * m_ScaleAmount, 0);
            if (scaleDim.Contains("x"))
            {
                newPos.x = newPos.y;
                newPos.y = 0;
            }

            GameObject newImage = Instantiate(m_OriginalImage, newPos, Quaternion.identity);
            newImage.transform.SetParent(transform, false);
            // newImageRect.position = newPos;
            // newImageRect.rotation = Quaternion.identity;
        }
        // Destroy if decrease size of line
        else if (count > numMarkers + 1)
        {
            Destroy(transform.GetChild((int)childCount - 1).gameObject); // TODO: maybe better way then creating and destroying these all the time
        }


        if (m_FollowCamera)
        {
            // follow camera angle
            Vector3 z = Camera.main.transform.position - m_RectTransform.position;
            Vector3 x = Vector3.Project(z, m_Parent.transform.right);
            Vector3 newZ = Vector3.Project(z, m_Parent.transform.forward);

            m_RectTransform.rotation = Quaternion.LookRotation((x + newZ).normalized, m_Parent.transform.up);
        }
    }

    void NOP(string _)
    {
    }

    private void UpdateXScale(ref Vector3 scale)
    {
        if (m_Parent.transform.parent.localScale.x != 0)
        {
            scale.x = m_BaseScale.x / (m_Volume && !(gameObject.name.ToLower() == "top" || gameObject.name.ToLower() == "bottom") ? m_Parent.transform.localScale.z : m_Parent.transform.localScale.x);
        }
        else
        {
            scale.x = 0;
        }
    }

    private void UpdateYScale(ref Vector3 scale) // pass by reference
    {
        if (m_Parent.transform.localScale.y != 0)
        {
            scale.y = m_BaseScale.y / (m_Volume && (gameObject.name.ToLower() == "front" || gameObject.name.ToLower() == "back") ? m_Parent.transform.localScale.x : m_Parent.transform.localScale.y);
        }
        else
        {
            scale.y = 0;
        }
    }

    private void UpdateXSize(ref Vector3 size)
    {
        size.x = m_BaseWidthHeight.x * (m_Volume && !(gameObject.name.ToLower() == "top" || gameObject.name.ToLower() == "bottom") ? m_Parent.transform.localScale.z : m_Parent.transform.localScale.x);

    }

    private void UpdateYSize(ref Vector3 size)
    {
        size.y = m_BaseWidthHeight.y * (m_Volume && (gameObject.name.ToLower() == "front" || gameObject.name.ToLower() == "back") ? m_Parent.transform.localScale.x : m_Parent.transform.localScale.y);

    }

    private void AdjustTicksX(RectTransform tick, Vector3 size)
    {
        Vector3 tickSize = tick.sizeDelta;
        tickSize.y = size.y;
        tick.sizeDelta = tickSize;
    }

    private void AdjustCrossAxisTicks()
    {
        Vector3 scale = m_RectTransform.localScale;
        UpdateYScale(ref scale);
        m_RectTransform.localScale = scale;

        Vector3 size = m_RectTransform.sizeDelta;
        UpdateYSize(ref size);
        m_RectTransform.sizeDelta = size;

        CrossAxisTickMarks.Add(m_CrossAxisOriginalImage.GetComponent<RectTransform>());

        float numMarkers = Mathf.Floor(size.y / m_ScaleAmount); // TODO: change 250 to something that is variable (will be different for line vs area)
        for (int i = 1; i <= numMarkers; ++i)
        {
            Vector3 newPos = new Vector3(0, i * m_ScaleAmount, 0);
            GameObject newImage = Instantiate(m_CrossAxisOriginalImage, newPos, Quaternion.identity);
            newImage.transform.SetParent(transform, false);

            CrossAxisTickMarks.Add(newImage.GetComponent<RectTransform>());
        }

        AdjustTicksX(m_OriginalImage.GetComponent<RectTransform>(), size);
    }

    void SetInitial()
    {
        if (gameObject.name.ToLower() == "top" || gameObject.name.ToLower() == "bottom")
        {
            Vector3 scale = m_RectTransform.localScale;
            UpdateXScale(ref scale);
            m_RectTransform.localScale = scale;

            Vector3 size = m_RectTransform.sizeDelta;
            UpdateXSize(ref size);
            m_RectTransform.sizeDelta = size;

            float numMarkers = Mathf.Floor(size.x / m_ScaleAmount); // TODO: change 250 to something that is variable (will be different for line vs area)
            for (int i = 1; i <= numMarkers; ++i)
            {
                Vector3 newPos = new Vector3(i * m_ScaleAmount, 0, 0);
                GameObject newImage = Instantiate(m_OriginalImage, newPos, Quaternion.identity);
                newImage.transform.SetParent(transform, false);

            }

            CrossAxisTickMarks.ForEach((RectTransform tick) => {
                Vector3 tickSize = tick.sizeDelta;
                tickSize.x = size.x;
                tick.sizeDelta = tickSize;
            });

        }
    }

    void RotateForVolume()
    {
        float result = m_VolumeForwardController.Result();
        if (result > 0.000001 && !m_VolumeForwardController.rotated.Contains(gameObject.name.ToLower())) {

            if (gameObject.name.ToLower() == "front" || gameObject.name.ToLower() == "back" || gameObject.name.ToLower() == "rside" || gameObject.name.ToLower() == "lside")
            {
                Vector3 rot = m_RectTransform.rotation.eulerAngles;
                rot.z += 180;
                m_RectTransform.rotation = Quaternion.Euler(rot);

                m_VolumeForwardController.rotated.Add(gameObject.name.ToLower());
            }

            if (m_VolumeForwardController.rotated.Count == 4) {
                m_VolumeForwardController.ZDirection = -m_VolumeForwardController.ZDirection;
                m_VolumeForwardController.rotated.Clear();
            }
        }
    }

    void CreateGridForMesh()
    {
        
    }
}
