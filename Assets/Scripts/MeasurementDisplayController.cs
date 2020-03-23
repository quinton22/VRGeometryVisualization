using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeasurementDisplayController : MonoBehaviour
{
    private struct Display
    {
        public Display(GameObject t, GameObject image)
        {
            m_Text = t;
            m_Image = image;
            parentRect = image.transform.parent.GetComponent<RectTransform>();
            text = t.GetComponent<Text>();
            imageRect = image.GetComponent<RectTransform>();

        }

        public static Display FromParent(GameObject parent)
        {
            return new Display(parent.transform.Find("Text").gameObject, parent.transform.Find("Image").gameObject);
        }

        public void Reset()
        {
            text.text = "";
            imageRect.sizeDelta = new Vector2(100, 0);
        }

        public void SetText(string s)
        {
            text.text = s;
        }

        public void SetImageHeight(float h)
        {
            imageRect.sizeDelta = new Vector2(100, h);
        }

        public void SetPos(float x, float y)
        {
            parentRect.anchoredPosition = new Vector2(x, y);
        }
        public GameObject m_Text { get; }
        public GameObject m_Image { get; }
        private RectTransform parentRect;
        private Text text;
        private RectTransform imageRect;
    }
    private List<string> ActiveList = new List<string>(); // list of active measurements
    const float yPos = 20;
    [SerializeField] private GameObject LengthDisplayObject;
    [SerializeField] private GameObject AreaDisplayObject;
    [SerializeField] private GameObject VolumeDisplayObject;
    private Display LengthDisplay;
    private Display AreaDisplay;
    private Display VolumeDisplay;
    private InputController m_InputController;
    private PointerController m_PointerController;
    private GameObject lastCollidingObject = null;

    void Start()
    {
        LengthDisplay = Display.FromParent(LengthDisplayObject);
        AreaDisplay = Display.FromParent(AreaDisplayObject);
        VolumeDisplay = Display.FromParent(VolumeDisplayObject);

        m_InputController = GameObject.Find("ToolController").GetComponent<InputController>();
        m_PointerController = GameObject.Find("Pointer").GetComponent<PointerController>();
    }

    void Update()
    {
        if (m_InputController.m_CurrentTool == InputController.Tool.None && m_PointerController.collidingObject != null)
        {
            if (!m_PointerController.collidingObject.Equals(lastCollidingObject))
            {
                lastCollidingObject = m_PointerController.collidingObject;

                if (lastCollidingObject.name.Contains("Line"))
                {
                    ActiveList.Add("length");
                    SetupPositions(lastCollidingObject);
                }
                else if (lastCollidingObject.name.Contains("Volume"))
                {
                    ActiveList.Add("Volume");
                    SetupPositions(lastCollidingObject);
                }
                else if (lastCollidingObject.transform.parent.name.Contains("Area"))
                {
                    ActiveList.Add("Area");
                    SetupPositions(lastCollidingObject);
                }
                
            }
        }
        else if (ActiveList.Count != 0)
        {
            lastCollidingObject = null;
            ClearActive();
            ResetAllValues();
        }
    }

    void ClearActive()
    {
        ActiveList.Clear();
    }

    void ResetAllValues()
    {
        LengthDisplay.Reset();
        AreaDisplay.Reset();
        VolumeDisplay.Reset();
    }

    void SetupPositions(GameObject obj)
    {
        if (ActiveList.Count == 1)
        {
            Display temp = GetDisplayFromText(ActiveList[0]);
            float s = GetMeasurement(ActiveList[0], obj);
            temp.SetImageHeight(s * 50);
            temp.SetPos(0, yPos);
            temp.SetText(ActiveList[0] + $": {s}");
        }
        else if (ActiveList.Count == 2)
        {

            Display temp;
            for (float i = -100; i <= 100; i += 200)
            {
                int j = (int)Mathf.Ceil(i / 200);
                temp = GetDisplayFromText(ActiveList[j]);
                float s = GetMeasurement(ActiveList[0], obj);
                temp.SetImageHeight(s * 50);
                temp.SetPos(i, yPos);
                temp.SetText(ActiveList[j] + $": {s}");
            } 
            // position of first is (-100, 20)
            // position of second is (100, 20)
        }
        else if (ActiveList.Count == 3)
        {
            Display temp;
            for (float i = -200; i <= 200; i += 200)
            {
                int j = (int)i / 200 + 1;
                temp = GetDisplayFromText(ActiveList[j]);
                float s = GetMeasurement(ActiveList[0], obj);
                temp.SetImageHeight(s * 50);
                temp.SetPos(i, yPos);
                temp.SetText(ActiveList[j] + $": {s}");
            }
            // position of first is (-200, 20)
            // position of second is (0, 20)
            // position of third is (200, 20)
        }
    }

    Display GetDisplayFromText(string name)
    {
        switch(name.ToLower())
        {
            case "length":
                return LengthDisplay;
            case "area":
                return AreaDisplay;
            case "volume":
                return VolumeDisplay;
            default:
                throw new System.Exception($"No display of the name '{name}'");
        }
    }

    float GetMeasurement(string name, GameObject obj)
    {
        switch (name.ToLower())
        {
            case "length":
                return obj.transform.localScale.y / .25f;
            case "area":
                return obj.transform.parent.localScale.x * obj.transform.parent.localScale.y / .25f;
            case "volume":
                return obj.transform.localScale.x * obj.transform.localScale.y * obj.transform.localScale.z / .125f;
            default:
                throw new System.Exception($"No display of the name '{name}'");
        }
    }
}
