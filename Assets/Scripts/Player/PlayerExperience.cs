using System;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerExperience : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _maxLevel = 5;
    [SerializeField] private int _expPerLevelStep = 5;

    private int _currentLevel = 1;
    private int _currentExp;

    public int CurrentLevel => _currentLevel;
    public event Action<int, int> OnExpChanged;
    public event Action<int> OnLevelUp;

    private void Start()
    {
        OnExpChanged?.Invoke(_currentExp, GetExpNeededForNextLevel());
    }

    public void AddExperience(int amount)
    {
        if(_currentLevel >= _maxLevel) return;

        _currentExp += amount;
        int needed = GetExpNeededForNextLevel();

        if(_currentExp >= needed)
        {
            _currentExp -= needed;
            LevelUp();
            needed = GetExpNeededForNextLevel();
        }

        OnExpChanged?.Invoke(_currentExp, needed);
    }
    
    private void LevelUp()
    {
        _currentLevel++;
        OnLevelUp?.Invoke(_currentLevel);

        if(_currentLevel >= _maxLevel)
        {
            _currentExp = 0;
        }
    }
    private int GetExpNeededForNextLevel()
    {
        return _currentLevel * _expPerLevelStep;
    }
}
