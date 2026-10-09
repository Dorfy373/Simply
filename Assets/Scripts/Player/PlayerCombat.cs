using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private PlayerAnimator _playerAnimator;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private PlayerWeaponSocket _weaponSocket;
    [SerializeField] private PlayerEquipment _playerEquipment;
    [SerializeField] private PlayerHealth _playerHealth;

    [Header("Hit Reaction")]
    [SerializeField] private float _hitStunDuration = 0.5f;

    [Header("Combo Settings")]
    [SerializeField] private int _maxComboSteps = 3;
    [SerializeField] private int[] _comboDamage = { 2, 5, 3 };

    private WeaponHitbox _weaponHitbox;

    private bool _isAttacking;
    private int _comboStep;
    private bool _comboInputBuffered;
    private bool _isStunned;

    private void OnEnable()
    {
        _inputReader.AttackPerformed += HandleAttack;
    }

    private void Start()
    {
        SetupCurrentWeapon();
    }

    private void SetupCurrentWeapon()
    {
        if (_playerEquipment == null)
        {
            return;
        }

        if (!_playerEquipment.HasWeapon)
        {
            return;
        }

        SetupWeaponHitbox();
    }

    private void SetupWeaponHitbox()
    {
        GameObject weaponInstance = _playerEquipment.CurrentWeaponInstance;

        if (weaponInstance == null)
        {
            return;
        }

        _weaponHitbox = weaponInstance.GetComponentInChildren<WeaponHitbox>();

        if (_weaponHitbox == null)
        {
            return;
        }

        _weaponHitbox.SetOwner(transform.root);
    }

    private void HandleAttack()
    {
        if (_playerHealth.IsDead) return;
        if (_isStunned) return;

        if (!_playerEquipment.HasWeapon)
        {
            return;
        }

        if (_weaponHitbox == null)
        {
            SetupWeaponHitbox();

            if (_weaponHitbox == null)
                return;
        }

        if (!_isAttacking)
            StartCombo();
        else
            _comboInputBuffered = true;
    }

    private void StartCombo()
    {
        _isAttacking = true;
        _comboStep = 1;
        _comboInputBuffered = false;

        _playerAnimator.TriggerAttack(_comboStep);
        _playerMovement.SetAttacking(true);
    }

    public void EnableAttackHitbox()
    {
        if (_weaponHitbox == null) return;

        if (_comboStep <= 0 || _comboStep > _comboDamage.Length)
            return;

        int damage = _comboDamage[_comboStep - 1];

        _weaponHitbox.EnableHitbox(damage);
    }

    public void DisableAttackHitbox()
    {
        if (_weaponHitbox == null) return;

        _weaponHitbox.DisableHitbox();
    }

    public void OnDrawSword()
    {
        SetupWeaponHitbox();
    }

    public void OnAttackEnd()
    {
        if (_playerHealth.IsDead) return;

        if (_comboInputBuffered && _comboStep < _maxComboSteps)
        {
            ContinueCombo();
        }
        else
        {
            EndCombo();
        }
    }

    private void ContinueCombo()
    {
        _comboStep++;
        _comboInputBuffered = false;

        _playerAnimator.TriggerAttack(_comboStep);
    }

    private void EndCombo()
    {
        _isAttacking = false;
        _comboStep = 0;

        _playerMovement.SetAttacking(false);
    }

    public void ReactToHit()
    {
        _isAttacking = false;
        _comboStep = 0;
        _comboInputBuffered = false;

        DisableAttackHitbox();

        _playerMovement.SetAttacking(false);

        StopCoroutine(nameof(HitStunRoutine));
        StartCoroutine(HitStunRoutine());
    }

    private IEnumerator HitStunRoutine()
    {
        _isStunned = true;

        yield return new WaitForSeconds(_hitStunDuration);

        _isStunned = false;
    }

    private void OnDisable()
    {
        _inputReader.AttackPerformed -= HandleAttack;
    }
}