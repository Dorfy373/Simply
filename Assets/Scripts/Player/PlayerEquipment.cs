using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInventory _playerInventory;
    [SerializeField] private PlayerWeaponSocket _weaponSocket;
    [SerializeField] private InputReader _inputReader;

    private GameObject[] _weaponInstances = new GameObject[2];

    private int _activeWeaponSlot = -1;

    public int ActiveWeaponSlot => _activeWeaponSlot;

    public bool HasWeapon => _activeWeaponSlot >= 0;

    public WeaponData CurrentWeaponData
    {
        get
        {
            if (_activeWeaponSlot < 0)
                return null;

            return _playerInventory.weaponSlots[_activeWeaponSlot];
        }
    }

    public GameObject CurrentWeaponInstance
    {
        get
        {
            if (_activeWeaponSlot < 0)
                return null;

            return _weaponInstances[_activeWeaponSlot];
        }
    }

    private void Start()
    {
        CreateAllWeapons();
    }

    private void OnEnable()
    {
        if (_inputReader == null) return;

        _inputReader.SelectWeapon1Performed += SelectWeapon1;
        _inputReader.SelectWeapon2Performed += SelectWeapon2;
    }

    private void CreateAllWeapons()
    {
        for (int i = 0; i < _playerInventory.weaponSlots.Length; i++)
        {
            WeaponData weaponData = _playerInventory.weaponSlots[i];

            if (weaponData == null)
                continue;

            _weaponInstances[i] = _weaponSocket.EquipWeapon(weaponData);

        }
    }

    private void SelectWeapon1()
    {
        ToggleWeapon(0);
    }

    private void SelectWeapon2()
    {
        ToggleWeapon(1);
    }

    private void ToggleWeapon(int slotIndex)
    {
        if (slotIndex < 0 ||
            slotIndex >= _playerInventory.weaponSlots.Length)
            return;

        WeaponData weaponData =
            _playerInventory.weaponSlots[slotIndex];

        if (weaponData == null)
        {
            return;
        }

        // Если нажали на уже активное оружие —
        // убираем его в хранилище
        if (_activeWeaponSlot == slotIndex)
        {
            PutCurrentWeaponAway();
            return;
        }

        // Если другое оружие уже было в руке —
        // сначала убираем его
        if (_activeWeaponSlot != -1)
        {
            PutCurrentWeaponAway();
        }

        // Достаём новое
        DrawWeapon(slotIndex);
    }

    private void DrawWeapon(int slotIndex)
    {
        GameObject weaponInstance = _weaponInstances[slotIndex];
        WeaponData weaponData = _playerInventory.weaponSlots[slotIndex];

        if (weaponInstance == null || weaponData == null)
            return;

        _weaponSocket.AttachToHand(
            weaponInstance,
            weaponData
        );

        _activeWeaponSlot = slotIndex;

    }

    private void PutCurrentWeaponAway()
    {
        if (_activeWeaponSlot == -1)
            return;

        GameObject weaponInstance =
            _weaponInstances[_activeWeaponSlot];

        WeaponData weaponData =
            _playerInventory.weaponSlots[_activeWeaponSlot];

        if (weaponInstance != null && weaponData != null)
        {
            _weaponSocket.AttachToStorage(
                weaponInstance,
                weaponData
            );
        }


        _activeWeaponSlot = -1;
    }

    private void OnDisable()
    {
        if (_inputReader == null) return;

        _inputReader.SelectWeapon1Performed -= SelectWeapon1;
        _inputReader.SelectWeapon2Performed -= SelectWeapon2;
    }
}
