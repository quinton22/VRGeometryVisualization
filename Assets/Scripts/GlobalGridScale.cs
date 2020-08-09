using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GlobalGridScale : Singleton<GlobalGridScale>
{

     // (Optional) Prevent non-singleton constructor use.
    protected GlobalGridScale() { }

    private static UnityEvent m_UpdateValues = new UnityEvent();
    
    public bool GridOn = true;

    [Range(1, 30), SerializeField] 
    private int m_GridScale = 20;
    public int GridScale {
        get { return m_GridScale; }
        set { m_GridScale = value; UpdateValues(); }
    }
    public static void AddScaleListener(UnityAction updateScaleCallback)
    {
        m_UpdateValues.AddListener(updateScaleCallback);
    }

    public static void RemoveScaleListener(UnityAction updateScaleCallback)
    {
        m_UpdateValues.RemoveListener(updateScaleCallback);
    }

    public void UpdateValues()
    {
        m_UpdateValues.Invoke();
    }

    void OnValidate()
    {
        if (GridScale < 0)
            GridScale = 0;
        else
            UpdateValues();
    }

    void OnDestroy()
    {
        m_UpdateValues.RemoveAllListeners();
    }

    public void ToggleGridOn()
    {
        GridOn = !GridOn;
    }
}
