using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yarn : MonoBehaviour
{
    public string colorName; // "Red", "Green", "Blue"

    public void SetColor(string name)
    {
        colorName = name;
    }
}
