using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ToolTypeList : MonoBehaviour
{
    public List<ToolType> List {
        get { return list; }
    }
    private List<ToolType> list = new List<ToolType>();

    public bool Add(ToolType item)
    {
        if (!list.Select(tool => tool.Name).Contains(item.Name))
        {
            list.Add(item);
            return true;
        }
        return false;
    }
}
