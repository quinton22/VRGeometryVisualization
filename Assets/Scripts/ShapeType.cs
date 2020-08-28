using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShapeTypeEnum {
    None,
    Line,
    Area,
    Volume,
    Sphere,
    Polygon
}

public class ShapeType : MonoBehaviour
{
   public ShapeTypeEnum m_ShapeType;
}
