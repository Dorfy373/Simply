using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [Header("Base Info")]
    public string itemName;
    public Sprite icon;

    [Tooltip("A prefab located in the game world (an item that can be picked up)")]
    public GameObject worldPrefab;
}
