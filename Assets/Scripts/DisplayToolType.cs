using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DisplayToolType : MonoBehaviour
{
    public float animationDuration = 1f;
    public float degreeRotation = 130;
    private Dictionary<Tool, GameObject> displayTools = new Dictionary<Tool, GameObject>();
    private Tool currentDisplayedTool = Tool.None;
    private float rotateOutTime = -1;
    private GameObject rotateOutGO;
    private float rotateInTime = -1;
    private GameObject rotateInGO;

    void Awake() {
        List<Tool> tools = new List<Tool>(Enum.GetValues(typeof(Tool)) as Tool[]);

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

            child.rotation = Quaternion.Euler(0, 0, -130);
        }
    }

    void Update()
    {
        if (rotateOutTime >= 0 && rotateOutTime <= animationDuration)
        {
            SpinOut(rotateOutTime / animationDuration);
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
            SpinIn(rotateInTime / animationDuration);
            rotateInTime += Time.deltaTime;
        }
        else if (rotateInTime > animationDuration)
        {
            rotateInTime = -1;
        }
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
    }

    private void StartDisplaying(GameObject obj)
    {
        rotateInGO = obj;
        rotateInGO.transform.rotation = Quaternion.Euler(0, 0, -130);
        rotateInGO.SetActive(true);
    }

    // TODO: move these to own class?
    private void SpinOut(float percentEllapsed)
    {
        rotateOutGO.transform.Rotate(0, 0, percentEllapsed * degreeRotation);
    }

    private void SpinIn(float percentEllapsed)
    {
        rotateOutGO.transform.Rotate(0, 0, percentEllapsed * degreeRotation);
    }
}
