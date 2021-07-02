using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DisplayToolType : MonoBehaviour
{
    public List<Tool> availableTools {
        get { return _availableTools; }
    }
    private List<Tool> _availableTools = new List<Tool>();
    public float animationDuration = 1f;
    public float degreeRotation = 130;
    private Dictionary<Tool, GameObject> displayTools = new Dictionary<Tool, GameObject>();
    private Tool currentDisplayedTool = Tool.None;
    private float rotateOutTime = -1;
    private GameObject rotateOutGO;
    private float rotateInTime = -1;
    private GameObject rotateInGO;
    private RectTransform rotateOutRect;
    private RectTransform rotateInRect;

    void Awake() {
        List<Tool> tools = new List<Tool>(Enum.GetValues(typeof(Tool)) as Tool[]);;

        foreach (Transform child in transform)
        {
            foreach (Tool tool in tools)
            {
                if (child.name.ToLower().Contains(tool.ToString().ToLower()))
                {
                    displayTools.Add(tool, child.gameObject);
                    tools.Remove(tool);
                    break;
                }   
            }

            if (!child.name.ToLower().Contains(currentDisplayedTool.ToString().ToLower()))
            {
                child.GetComponent<RectTransform>().Rotate(new Vector3(0, 0, -130));
                child.gameObject.SetActive(false);
            }
            
        }

        _availableTools = new List<Tool>(displayTools.Keys);
    }

    void Update()
    {
        if (rotateOutTime >= 0 && rotateOutTime <= animationDuration)
        {
            SpinOut(Time.deltaTime / animationDuration);
            rotateOutTime += Time.deltaTime;
            if (rotateOutTime >= 0.5 * animationDuration && rotateInTime < 0)
            {
                rotateInTime = 0;
            }
        }
        else if (rotateOutTime > animationDuration)
        {
            rotateOutTime = -1;
            rotateOutGO.SetActive(false);
            rotateOutGO = null;
        }

        if (rotateInTime >= 0 && rotateInTime <= animationDuration) {
            SpinIn(Time.deltaTime / animationDuration);
            rotateInTime += Time.deltaTime;
        }
        else if (rotateInTime > animationDuration)
        {
            rotateInTime = -1;
        }
    }

    public void DisplayTriggerPull()
    {

    }

    public void DisplayButtonPress()
    {

    }

    public void DisplayTool(Tool tool)
    {
        // spin out current tool
        StopDisplaying(displayTools[currentDisplayedTool]);
        // spin in new tool
        StartDisplaying(displayTools[tool]);
        // set currentTool
        currentDisplayedTool = tool;
    }

    // TODO: override these for different pens?
    private void StopDisplaying(GameObject obj)
    {
        rotateOutTime = 0;
        rotateOutGO = obj;
        rotateOutRect = obj.GetComponent<RectTransform>();
    }

    private void StartDisplaying(GameObject obj)
    {
        rotateInGO = obj;
        rotateInRect = obj.GetComponent<RectTransform>();
        rotateInRect.localRotation = Quaternion.Euler(0, 0, -degreeRotation);
        rotateInGO.SetActive(true);
    }

    // TODO: move these to own class?
    private void SpinOut(float percentChanged)
    {
        rotateOutRect.Rotate(new Vector3(0, 0, percentChanged * degreeRotation));
    }

    private void SpinIn(float percentChanged)
    {
         rotateInRect.Rotate(new Vector3(0, 0, percentChanged * degreeRotation));
    }
}
