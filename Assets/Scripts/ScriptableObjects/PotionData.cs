using UnityEngine;

public enum PotionType
{
    Health,
    Energy
}

[CreateAssetMenu(fileName = "PotionData", menuName = "Items/PotionData")]
public class PotionData : ItemData
{
    [Header("Potion Settings")]
    public PotionType potionType;

    [Tooltip("How much does it restore?")]
    public float amount = 30f;

    [Header("Stacking")]
    public bool isStackable = true;
    public int maxStack = 50;
}
