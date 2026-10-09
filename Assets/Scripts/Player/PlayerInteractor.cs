using System;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private LayerMask _pickupableLayer;
    [SerializeField] private float _searchRadius = 5f;
   
    [SerializeField] private float _checkInterval = 0.1f;

    [Header("References")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private PlayerInventory _playerInventory; 

    private Pickupable _currentTarget;
    private float _checkTimer;
    public event Action<Transform> OnTargetInRange;
    public event Action OnTargetOutOfRange;

    private void OnEnable()
    {
        _inputReader.InteractPerformed += HandleInteractPerformed;
    }


    private void Update()
    {
        _checkTimer -= Time.deltaTime;
        if (_checkTimer <= 0f)
        {
            _checkTimer = _checkInterval;
            FindNearestPickupable();
        }
    }

    private void FindNearestPickupable()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _searchRadius, _pickupableLayer);

        Pickupable closest = null;
        float closestDistance = float.MaxValue;

        foreach (var hit in hits)
        {
            var pickupable = hit.GetComponent<Pickupable>();
            if (pickupable == null) continue;

            float distance = Vector3.Distance(transform.position, pickupable.transform.position);

            if (distance <= pickupable.InteractRadius && distance < closestDistance)
            {
                closest = pickupable;
                closestDistance = distance;
            }
        }

      
        if (closest != _currentTarget)
        {
            _currentTarget = closest;

            if (_currentTarget != null)
                OnTargetInRange?.Invoke(_currentTarget.transform);
            else
                OnTargetOutOfRange?.Invoke();
        }
    }

    private void HandleInteractPerformed()
    {
        if (_currentTarget == null) 
        {
            return;
        }
        bool wasAdded = _playerInventory.TryAddItem(_currentTarget.ItemData);
        if (wasAdded)
        {
            Pickupable pickedUpItem = _currentTarget;

            _currentTarget = null;

            OnTargetOutOfRange?.Invoke();

            pickedUpItem.OnPickedUp();
        }
       
    }
    private void OnDisable()
    {
        _inputReader.InteractPerformed -= HandleInteractPerformed;
    }
}
