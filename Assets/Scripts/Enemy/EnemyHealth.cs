using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Components")]
    [SerializeField] private EnemyAnimator _animator;
    [SerializeField] private EnemyCombat _combat;
    private PlayerExperience _playerExperience;

    [Header("Settings")]
    [SerializeField] private int _maxHealth = 20;

    [Header("Experience")]
    [SerializeField] private int _expReward = 5;

    private int _currentHealth;
    public bool _isDead;

    public event Action OnDeath;
    
    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void Initialize(PlayerExperience playerExperience)
    {
        _playerExperience = playerExperience;
    }

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;

        if (_currentHealth <= 0)
        {
            _isDead = true;
            _playerExperience?.AddExperience(_expReward);
            OnDeath?.Invoke();
        }
        else
        {
            _animator.TriggerHit();
            _combat.ReactToHit();
        }
    }
}
