using UnityEngine;

public enum HandSocketType { RightHand, LeftHand }
public enum StorageSocketType { Back, Belt, Hip }


[CreateAssetMenu(fileName = "New Weapon", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ItemData
{

    [Header("Weapon Prefab")]
    public GameObject weaponPrefab;

    [Header("Sockets")]
    public HandSocketType handSocket;
    public StorageSocketType storageSocket;

    [Header("Hand Offset")]
    public Vector3 handPositionOffset;
    public Vector3 handRotationOffset;

    [Header("Storage Offset")]
    public Vector3 storagePositionOffset;
    public Vector3 storageRotationOffset;
}
