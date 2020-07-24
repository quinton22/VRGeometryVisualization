using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SliderController : MonoBehaviour
{
    public Text SliderText;
    public float limit;
    private int[] range = new int[]{1, 20};
   private GlobalGridScale globalGridScale;
   // TODO: find some programmatic way to get total scale, rn it's just 1-20 hardcoded here

   void Awake()
   {
       globalGridScale = FindObjectOfType<GlobalGridScale>();
       SliderText.text = $"Scale: {globalGridScale.GridScale}";
       SetSlider();
   }

   void SetSlider()
   {
       Vector3 pos = transform.localPosition;
       pos.x = ((globalGridScale.GridScale - 1) / (range[1] - range[0]) * 2 - 1) * limit;
       transform.localPosition = pos;
   }

   public void ButtonPress(int add)
   {
       Vector3 pos = transform.localPosition;
       pos.x += ((add * limit * 2) / (range[1] - range[0]));
        if (Mathf.Abs(pos.x) > limit) {
            pos.x = limit * add;
        }
       transform.localPosition = pos;
       UpdateGrid();
   }

   void UpdateGrid()
   {
       globalGridScale.GridScale = (int) ((transform.localPosition.x / (limit) + 1) / 2f * (range[1] - range[0]) + 1);
       SliderText.text = $"Scale: {globalGridScale.GridScale}";
   }

}
