using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public bool pegavel;
    public AudioClip audioClip;
    public string text;
    public Sprite image;

    [Header("Inventario")]
    public bool InvetoryItem;
    public string CollectMsg;
    public string InvetoryTxt;
}