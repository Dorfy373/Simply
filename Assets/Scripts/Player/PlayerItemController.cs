using UnityEngine;

public class PlayerItemController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private PlayerInventory _playerInventory;
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private PlayerEnergy _playerEnergy;

    private void OnEnable()
    {
        if (_inputReader == null) return;

        _inputReader.UsePotion1Performed += UsePotion1;
        _inputReader.UsePotion2Performed += UsePotion2;
    }

    private void UsePotion1()
    {
        UsePotion(0);
    }

    private void UsePotion2()
    {
        UsePotion(1);
    }

    private void UsePotion(int slotIndex)
    {
        if (_playerInventory == null)
            return;

        PotionSlot slot = _playerInventory.potionSlots[slotIndex];

        if (slot == null || slot.data == null || slot.count <= 0)
            return;

        PotionData potion = slot.data;

        bool canUse = false;

        switch (potion.potionType)
        {
            case PotionType.Health:
                canUse = _playerHealth.CurrentHealth < _playerHealth.MaxHealth;
                break;

            case PotionType.Energy:
                canUse = _playerEnergy.CurrentEnergy < _playerEnergy.MaxEnergy;
                break;
        }

        // Нельзя использовать зелье, если параметр уже на максимуме
        if (!canUse)
        {
            return;
        }

        // Только после проверки забираем зелье из инвентаря
        PotionData usedPotion = _playerInventory.UsePotion(slotIndex);

        if (usedPotion == null)
            return;

        ApplyPotionEffect(usedPotion);

    }

    private void ApplyPotionEffect(PotionData potion)
    {
        switch (potion.potionType)
        {
            case PotionType.Health:
                _playerHealth.RestoreHealth(Mathf.RoundToInt(potion.amount));
                break;

            case PotionType.Energy:
                _playerEnergy.RestoreEnergy(potion.amount);
                break;
        }
    }

    private void OnDisable()
    {
        if (_inputReader == null) return;

        _inputReader.UsePotion1Performed -= UsePotion1;
        _inputReader.UsePotion2Performed -= UsePotion2;
    }
}
