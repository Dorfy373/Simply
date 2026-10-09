using UnityEngine;
using UnityEngine.UIElements;

public class PlayerHUDController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private PlayerExperience _playerExperience;
    [SerializeField] private PlayerEnergy _playerEnergy;
    [SerializeField] private PlayerInventory _playerInventory;
    [SerializeField] private PlayerInteractor _playerInteractor;
    [SerializeField] private InputReader _inputReader;

    private VisualElement _weaponIcon1;
    private VisualElement _weaponIcon2;
    private VisualElement _potionIcon1;
    private VisualElement _potionIcon2;
    private VisualElement _interactPrompt;
    private VisualElement _healthFillBar;
    private VisualElement _expFillBar;
    private VisualElement _energyFillBar;
    private Button _dashButton;
    private Button _jumpButton;
    private Button _attackButton;
    private Button _runButton;
    private Label _levelNumber;
    private Label _potionCount1;
    private Label _potionCount2;

    private void OnEnable()
    {
        var root = _uiDocument.rootVisualElement;
        _healthFillBar = root.Q<VisualElement>("HealthFillBar");
        _expFillBar = root.Q<VisualElement>("ExpFillBar");
        _energyFillBar = root.Q<VisualElement>("EnergyFillBar");
        _dashButton = root.Q<Button>("DashButton");
        _jumpButton = root.Q<Button>("JumpButton");
        _attackButton = root.Q<Button>("AttackButton");
        _runButton = root.Q<Button>("RunButton");
        _levelNumber = root.Q<Label>("LevelNumber");
        _potionCount1 = root.Q<Label>("PotionCount1");
        _potionCount2 = root.Q<Label>("PotionCount2");
        _weaponIcon1 = root.Q<VisualElement>("WeaponIcon1");
        _weaponIcon2 = root.Q<VisualElement>("WeaponIcon2");
        _potionIcon1 = root.Q<VisualElement>("PotionIcon1");
        _potionIcon2 = root.Q<VisualElement>("PotionIcon2");
        _interactPrompt = root.Q<VisualElement>("InteractPrompt");

        _playerHealth.OnHealthChanged += UpdateHealthBar;
        _playerExperience.OnExpChanged += UpdateExpBar;
        _playerEnergy.OnEnergyChanged += UpdateEnergyBar;
        _playerExperience.OnLevelUp += UpdateLevelNumber;

        if (_playerInventory != null)
        {
            _playerInventory.OnInventoryChanged += UpdateInventoryUI;
        }

        if (_playerInteractor != null)
        {
            _playerInteractor.OnTargetInRange += ShowInteractPrompt;
            _playerInteractor.OnTargetOutOfRange += HideInteractPrompt;
        }

        if (_inputReader != null)
        {
            _inputReader.JumpPerformed += OnJumpPressed;
            _inputReader.JumpCanceled += OnJumpReleased;

            _inputReader.AttackPerformed += OnAttackPressed;
            _inputReader.AttackCanceled += OnAttackReleased;
        }
    }
    private void UpdateInventoryUI()
    {
        if (_playerInventory == null) return;

        UpdateWeaponSlot(_weaponIcon1, _playerInventory.weaponSlots[0]);
        UpdateWeaponSlot(_weaponIcon2, _playerInventory.weaponSlots[1]);

        UpdatePotionSlot(_potionIcon1, _potionCount1, _playerInventory.potionSlots[0]);
        UpdatePotionSlot(_potionIcon2, _potionCount2, _playerInventory.potionSlots[1]);
    }
    private void UpdateWeaponSlot(VisualElement slot, WeaponData weapon)
    {
        if (slot == null) return;

        if (weapon == null || weapon.icon == null)
        {
            slot.style.backgroundImage = StyleKeyword.None;
            return;
        }

        slot.style.backgroundImage = new StyleBackground(weapon.icon);
    }

    private void UpdatePotionSlot(VisualElement icon, Label countLabel, PotionSlot potionSlot)
    {
        if (icon == null) return;

        if (potionSlot == null || potionSlot.data == null || potionSlot.data.icon == null)
        {
            icon.style.backgroundImage = StyleKeyword.None;

            if (countLabel != null)
                countLabel.style.display = DisplayStyle.None;

            return;
        }

        icon.style.backgroundImage = new StyleBackground(potionSlot.data.icon);

        if (countLabel != null)
        {
            if (potionSlot.count > 1)
            {
                countLabel.text = potionSlot.count.ToString();
                countLabel.style.display = DisplayStyle.Flex;
            }
            else
            {
                // Если зелье одно — цифру можно не показывать (как в большинстве игр)
                countLabel.style.display = DisplayStyle.None;
            }
        }
    }
    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        var percent = (float)currentHealth / maxHealth;
        _healthFillBar.style.width = new StyleLength( new Length(percent * 88f, LengthUnit.Percent));
    }
    private void UpdateExpBar(int currentExp, int expNeeded)
    {
        float percent = (float)currentExp / expNeeded;
        _expFillBar.style.height = new StyleLength(new Length(percent * 67f, LengthUnit.Percent));
    }
    private void UpdateEnergyBar(float currentEnergy, float maxEnergy)
    {
        float percent = currentEnergy / maxEnergy;
        _energyFillBar.style.height = new StyleLength(new Length(percent * 67f, LengthUnit.Percent));
    }
    private void UpdateLevelNumber(int newLevel)
    {
        _levelNumber.text = newLevel.ToString();
    }
    private void OnJumpPressed() => SetButtonPressed(_jumpButton, true);
    private void OnJumpReleased() => SetButtonPressed(_jumpButton, false);

    private void OnAttackPressed() => SetButtonPressed(_attackButton, true);
    private void OnAttackReleased() => SetButtonPressed(_attackButton, false);
    private void SetButtonPressed(Button button, bool isPressed)
    {
        if (button == null) return;

        if (isPressed)
            button.AddToClassList("buttonPressed");
        else
            button.RemoveFromClassList("buttonPressed");
    }
    public void SetDashPressed(bool isPressed)
    {
        SetButtonPressed(_dashButton, isPressed);
    }

    public void SetRunPressed(bool isPressed)
    {
        SetButtonPressed(_runButton, isPressed);
    }
    private void ShowInteractPrompt(Transform target)
    {
        if (_interactPrompt == null) return;

        _interactPrompt.style.display = DisplayStyle.Flex;
    }

    private void HideInteractPrompt()
    {
        if (_interactPrompt == null) return;

        _interactPrompt.style.display = DisplayStyle.None;
    }
    private void OnDisable()
    {
        _playerHealth.OnHealthChanged -= UpdateHealthBar;
        _playerExperience.OnExpChanged -= UpdateExpBar;
        _playerEnergy.OnEnergyChanged -= UpdateEnergyBar;
        _playerExperience.OnLevelUp -= UpdateLevelNumber;

        if (_inputReader != null)
        {
            _inputReader.JumpPerformed -= OnJumpPressed;
            _inputReader.JumpCanceled -= OnJumpReleased;


            _inputReader.AttackPerformed -= OnAttackPressed;
            _inputReader.AttackCanceled -= OnAttackReleased;

        }
        if (_playerInteractor != null)
        {
            _playerInteractor.OnTargetInRange -= ShowInteractPrompt;
            _playerInteractor.OnTargetOutOfRange -= HideInteractPrompt;
        }
        if (_playerInventory != null)
            _playerInventory.OnInventoryChanged -= UpdateInventoryUI;
    }
}
