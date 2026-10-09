using System.Collections;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private EnemyAnimator _animator;
    [SerializeField] private WeaponHitbox _weaponHitbox;

    [Header("Settings")]
    [SerializeField] private int _damage = 5;
    [SerializeField] private float _attackCooldown = 2f;

    [Header("Hit Reaction")]
    [SerializeField] private float _hitStunDuration = 0.5f;

    private bool _isStunned;

    private float _lastAttackTime = -Mathf.Infinity;


    public bool CanAttack => !_isStunned && Time.time >= _lastAttackTime + _attackCooldown;

    private void Awake()
    {
        _weaponHitbox.SetOwner(transform.root);
    }
    public void Attack()
    {
        if (!CanAttack) return;

        _lastAttackTime = Time.time;
        _animator.TriggerAttack();
    }

    public void ReactToHit()
    {
        DisableAttackHitbox();
        StopCoroutine(nameof(HitStunRoutine));
        StartCoroutine(HitStunRoutine());
    }
    public void EnableAttackHitbox()
    {
        _weaponHitbox.EnableHitbox(_damage);
    }

    public void DisableAttackHitbox()
    {
        _weaponHitbox.DisableHitbox();
    }
    private IEnumerator HitStunRoutine()
    {
        _isStunned = true;
        yield return new WaitForSeconds(_hitStunDuration);
        _isStunned = false;
    }
}
