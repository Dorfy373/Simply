using System;
using UnityEngine;

[Serializable]
public class PotionSlot
{
    public PotionData data;
    public int count;
}
public class PlayerInventory : MonoBehaviour
{
    [Header("Weapon Slots")]
    public WeaponData[] weaponSlots = new WeaponData[2];

    [Header("Potion Slots")]
    public PotionSlot[] potionSlots = new PotionSlot[2] { new PotionSlot(), new PotionSlot() };

    // Один общий сигнал "что-то в инвентаре поменялось" —
    // HUD просто перерисует все 4 слота заново, не нужно
    // отслеживать, какой конкретно слот изменился
    public event Action OnInventoryChanged;

    private void Start()
    {
        // оружие уже назначено вручную в инспекторе — сообщаем HUD стартовое состояние
        OnInventoryChanged?.Invoke();
    }

    // Точка входа для PlayerInteractor — не знает, оружие это или зелье,
    // просто пробует добавить, а дальше сам разбирается по типу
    public bool TryAddItem(ItemData item)
    {
        if (item is WeaponData weapon)
            return TryAddWeapon(weapon);

        if (item is PotionData potion)
            return TryAddPotion(potion);

        return false;
    }

    private bool TryAddWeapon(WeaponData weapon)
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i] == null)
            {
                weaponSlots[i] = weapon;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        return false; // оба слота оружия заняты
    }

    private bool TryAddPotion(PotionData potion)
    {
        // Шаг 1: пробуем докинуть в уже существующий стак ТАКОГО ЖЕ зелья
        foreach (var slot in potionSlots)
        {
            if (slot.data == potion && potion.isStackable && slot.count < potion.maxStack)
            {
                slot.count++;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        // Шаг 2: подходящего стака нет — ищем полностью пустой слот
        foreach (var slot in potionSlots)
        {
            if (slot.data == null)
            {
                slot.data = potion;
                slot.count = 1;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        return false; // все слоты заняты и/или стаки полные
    }

    // Понадобится в пункте 5 (клавиши 3/4) — потратить одно зелье из слота
    public PotionData UsePotion(int slotIndex)
    {
        var slot = potionSlots[slotIndex];
        if (slot.data == null || slot.count <= 0) return null;

        var usedPotion = slot.data;
        slot.count--;

        if (slot.count <= 0)
            slot.data = null; // стак закончился — слот снова пустой

        OnInventoryChanged?.Invoke();
        return usedPotion;
    }
}
