using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GlobalGridScale : Singleton<GlobalGridScale>
{

     // (Optional) Prevent non-singleton constructor use.
    protected GlobalGridScale() { }

    private static UnityEvent m_UpdateValues = new UnityEvent();

    [Range(1, 10), SerializeField] 
    private int m_GridScale = 4;
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
        UpdateValues();
    }

    void OnDestroy()
    {
        m_UpdateValues.RemoveAllListeners();
    }
}
