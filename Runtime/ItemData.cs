using System;
using UnityEngine;

namespace Lookloop.ItemManager
{
[CreateAssetMenu(fileName = "ItemData", menuName = "ItemManager/ItemData")]
public class ItemTable : ScriptableObject
{
    public int Id;
    public int Type;
    public string ItemName;
    [TextArea(3, 10)]
    public string ItemDescription;
    public Sprite ItemSprite;
    public Sprite GlowSprite; 
}
}