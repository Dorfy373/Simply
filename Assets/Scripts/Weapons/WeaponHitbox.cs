using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class WeaponHitbox : MonoBehaviour
{
    private int _currentDamage;

    private Transform _owner;
    private BoxCollider _collider;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        _collider.enabled = false;
    }

    public void SetOwner(Transform owner)
    {
        _owner = owner;
    }

    public void EnableHitbox(int damage)
    {
        _currentDamage = damage;
        _collider.enabled = true;
    }

    public void DisableHitbox()
    {
        _collider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (_owner != null && other.transform.root == _owner)
            return;

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(_currentDamage);
        }
    }
}
