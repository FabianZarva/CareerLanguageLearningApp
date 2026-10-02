using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BackgroundItem", menuName = "Shop/Background Item", order = 0)]
public class BackgroundItem : ScriptableObject
{
    public string Id; 
    public Sprite Portrait;
    public Sprite Landscape;
    public Sprite Preview;
    public int Price;
}

