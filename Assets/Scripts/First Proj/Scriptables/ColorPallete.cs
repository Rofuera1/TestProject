using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Basic Color Pallete", menuName = "ColorPallete")]
public class ColorPallete : ScriptableObject
{
    public Color ColorGood;
    public Color ColorBad;
    public Color ColorNeutral;
}
