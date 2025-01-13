using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item ", menuName = "Item Parameters")]
public class ItemParams : ScriptableObject
{
    public string ItemName;
    public int ID;
    public int CostFromPlayer;
    public int CostFromMerchant;
    public Sprite Sprite;
}
