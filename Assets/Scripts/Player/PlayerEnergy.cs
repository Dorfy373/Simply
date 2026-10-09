using System;
using System.Collections;
using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _maxEnergy = 100f;
    [SerializeField] private float _drainRate = 20f;
    [SerializeField] private float _regenRate = 15f;
    [SerializeField] private float _regenDelay = 4f;

    private float _currentEnergy;
    private bool _isSpending;
    private bool _wasSpendingLastFrame;
    private bool _canRegen = true;
    private Coroutine _regenDelayRoutine;

    public bool HasEnergy => _currentEnergy > 0;
    public float CurrentEnergy => _currentEnergy; 
    public float MaxEnergy => _maxEnergy;
    public event Action<float, float> OnEnergyChanged;

    private void Awake()
    {
        _currentEnergy = _maxEnergy;
    }
    private void Start()
    {
        OnEnergyChanged?.Invoke(_currentEnergy, _maxEnergy);
    }
    private void Update()
    {
        float previous = _currentEnergy;
        if (_isSpending)
        {
            if(_regenDelayRoutine != null)
            {
                StopCoroutine(_regenDelayRoutine);
                _regenDelayRoutine = null;
            }
            _canRegen = false;
            _currentEnergy -= _drainRate * Time.deltaTime;
        }
        else
        {
            if (_wasSpendingLastFrame)
            {
                StartCoroutine(RegenDelayRoutine());
            }
            if (_canRegen)
                _currentEnergy += _regenRate * Time.deltaTime;
        }
        _currentEnergy = Mathf.Clamp(_currentEnergy, 0f, _maxEnergy);
        _wasSpendingLastFrame = _isSpending;

        if (!Mathf.Approximately(previous, _currentEnergy))
            OnEnergyChanged?.Invoke(_currentEnergy, _maxEnergy);
    }

    public void SetSpending(bool isSpending)
    {
        _isSpending = isSpending;
    }

    public void RestoreEnergy(float amount)
    {
        _currentEnergy += amount;
        _currentEnergy = Mathf.Clamp(_currentEnergy, 0f, _maxEnergy);

        OnEnergyChanged?.Invoke(_currentEnergy, _maxEnergy);

    }
    private IEnumerator RegenDelayRoutine()
    {
        yield return new WaitForSeconds(_regenDelay);
        _canRegen = true;
        _regenDelayRoutine = null;
    }
}
