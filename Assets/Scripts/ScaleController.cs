using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleController : MonoBehaviour
{
    private GameObject m_Parent;
    private RectTransform m_RectTransform;
    private Vector3 m_BaseScale;
    private Vector2 m_BaseWidthHeight;
    private GameObject m_OriginalImage;
    // Start is called before the first frame update
    void Start()
    {
        m_Parent = transform.parent.gameObject;
        m_RectTransform = gameObject.GetComponent<RectTransform>();
        m_BaseScale = m_RectTransform.localScale;
        m_BaseWidthHeight = m_RectTransform.sizeDelta;
        m_OriginalImage = transform.Find("Image").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Parent.name.Contains("Clone"))
        {
            Vector3 scale = m_RectTransform.localScale;
            if (m_Parent.transform.localScale.y != 0)
            {
                scale.y = m_BaseScale.y / m_Parent.transform.localScale.y;
            }
            else
            {
                scale.y = 0;
            }
            m_RectTransform.localScale = scale;

            Vector3 size = m_RectTransform.sizeDelta;
            size.y = m_BaseWidthHeight.y * m_Parent.transform.localScale.y;
            m_RectTransform.sizeDelta = size;

            float numMarkers = Mathf.Floor(size.y / 250); // TODO: change 250 to something that is variable (will be different for line vs area)
            float childCount = transform.childCount;
            if (childCount <  numMarkers + 1)
            {
                Vector3 newPos = new Vector3(0, numMarkers * 250, 0);
                GameObject newImage = Instantiate(m_OriginalImage, newPos, Quaternion.identity);
                newImage.transform.SetParent(transform, false);
                // newImageRect.position = newPos;
                // newImageRect.rotation = Quaternion.identity;
            }
            // Destroy if decrease size of line
            else if (childCount > numMarkers + 1)
            {
                Destroy(transform.GetChild((int) childCount - 1).gameObject); // TODO: maybe better way then creating and destroying these all the time
            }


            // follow camera angle
            Vector3 z = Camera.main.transform.position - m_RectTransform.position;
            Vector3 x = Vector3.Project(z, m_Parent.transform.right);
            Vector3 newZ = Vector3.Project(z, m_Parent.transform.forward);

            m_RectTransform.rotation = Quaternion.LookRotation((x + newZ).normalized, m_Parent.transform.up);
        }
        
    }
}
