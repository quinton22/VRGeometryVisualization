using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour
{
    private GameObject buttonBase;
    private GameObject button;
    private GameObject canvas;
    private Text text;

    public Material ButtonMaterial;
    public Material ButtonBaseMaterial;
    public Vector3 CanvasOffset = new Vector3(0, -0.024f, -0.125f);
    public string ButtonText = "";
    public TextAnchor Alignment = TextAnchor.UpperLeft;
    public int FontSize = 50;
    public Color TextColor = Color.black;

    void Awake() {
        GetChildObjects();
        UpdateValues();
    }

    void OnValidate()
    {
        GetChildObjects();
        UpdateValues();
    }

    void GetChildObjects()
    {
        buttonBase = transform.Find("ButtonHolder").gameObject;
        button = transform.Find("Button").gameObject;
        canvas = transform.Find("Canvas").gameObject;
        text = GetComponentInChildren<Text>();
    }

    void UpdateValues()
    {
        if (ButtonMaterial != null)
            button.GetComponent<Renderer>().sharedMaterial = ButtonMaterial;

        if (ButtonBaseMaterial != null)
            buttonBase.GetComponent<Renderer>().sharedMaterial = ButtonBaseMaterial;

        text.text = ButtonText;
        text.fontSize = FontSize;
        text.alignment = Alignment;
        text.color = TextColor;

       canvas.transform.localPosition = CanvasOffset;
    }
}
