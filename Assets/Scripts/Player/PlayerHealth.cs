using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IDamageable
{

    [Header("Components")]
    [SerializeField] private PlayerAnimator _playerAnimator;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private PlayerCombat _playerCombat;

    [Header("Settings")]
    [SerializeField] private int _maxHealth = 20;
    [SerializeField] private float _destroyDelay = 2f;
    private int _currentHealth;
    private bool _isInvulnerable;
    private bool _isDead;

    public bool IsDead => _isDead;
    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;
    public event Action<int, int> OnHealthChanged;
    public event Action OnDeath;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }
    private void Start()
    {
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
    }

    public void SetInvulnerable(bool value)
    {
        _isInvulnerable = value;
    }
    public void RestoreHealth(int amount)
    {
        if (_isDead) return;

        _currentHealth += amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);

        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

    }

    public void TakeDamage(int amount)
    {
        if (_isDead || _isInvulnerable) return;

        _currentHealth -= amount;
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

        if (_currentHealth <= 0)
        {
            _isDead = true;
            HandleDeath();
        }
        else
        {
            _playerAnimator.TriggerHit();
            _playerCombat.ReactToHit();
        }
    }

    private void HandleDeath()
    {
        _playerMovement.enabled = false;

        _playerAnimator.TriggerDeath();

        GetComponent<Collider>().enabled = false;

        OnDeath?.Invoke();

        StartCoroutine(LoadEndMenu());
    }
    private IEnumerator LoadEndMenu()
    {
        yield return new WaitForSeconds(_destroyDelay);

        SceneManager.LoadScene("EndMenu");
    }
}
