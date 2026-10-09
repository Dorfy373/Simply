using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponSocket : MonoBehaviour
{
    [Header("Hand Sockets")]
    [SerializeField] private Transform _rightHandSocket;
    [SerializeField] private Transform _leftHandSocket;

    [Header("Storage Sockets")]
    [SerializeField] private Transform _backSocket;
    [SerializeField] private Transform _beltSocket;
    [SerializeField] private Transform _hipSocket;

    private Dictionary<HandSocketType, Transform> _handSockets;
    private Dictionary<StorageSocketType, Transform> _storageSockets;

    private void Awake()
    {
        _handSockets = new Dictionary<HandSocketType, Transform>
        {
            { HandSocketType.RightHand, _rightHandSocket },
            { HandSocketType.LeftHand, _leftHandSocket }
        };

        _storageSockets = new Dictionary<StorageSocketType, Transform>
        {
            { StorageSocketType.Back, _backSocket },
            { StorageSocketType.Belt, _beltSocket },
            { StorageSocketType.Hip, _hipSocket }
        };
    }

    public GameObject EquipWeapon(WeaponData weaponData)
    {
        GameObject weaponInstance = Instantiate(weaponData.weaponPrefab);
        AttachToStorage(weaponInstance, weaponData);
        return weaponInstance;
    }

    public void AttachToHand(GameObject weaponInstance, WeaponData weaponData)
    {
        Transform socket = _handSockets[weaponData.handSocket];
        Attach(weaponInstance, socket, weaponData.handPositionOffset, weaponData.handRotationOffset);
        SetLevitationEnabled(weaponInstance, false);
    }
    public void AttachToStorage(GameObject weaponInstance, WeaponData weaponData)
    {
        Transform socket = _storageSockets[weaponData.storageSocket];
        Attach(weaponInstance, socket, weaponData.storagePositionOffset, weaponData.storageRotationOffset);
        SetLevitationEnabled(weaponInstance, true);
    }

    private void Attach(GameObject weaponInstance, Transform socket, Vector3 positionOffset, Vector3 rotationOffset)
    {

        if (weaponInstance == null)
        {
            return;
        }

        if (socket == null)
        {
            return;
        }
        weaponInstance.transform.SetParent(socket);
        weaponInstance.transform.localPosition = positionOffset;
        weaponInstance.transform.localRotation = Quaternion.Euler(rotationOffset);
    }

    private void SetLevitationEnabled(GameObject weaponInstance, bool enabled)
    {
        Animator levitationAnimator = weaponInstance.GetComponentInChildren<Animator>();

        if (levitationAnimator != null)
            levitationAnimator.enabled = enabled;
   
    }
}