using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "UIInputReader", menuName = "Input/UIInputReader")]
public class UIInputReader : ScriptableObject
{
    private InputSystem_Actions _inputActions;

    public event Action PausePerformed;

    private void OnEnable()
    {
        if (_inputActions == null)
        {
            _inputActions = new InputSystem_Actions();
            _inputActions.UI.Pause.performed += HandlePausePerformed;
        }

        _inputActions.UI.Enable();
    }
    private void OnDisable()
    {
        if (_inputActions == null)
            return;

        _inputActions.UI.Disable();
    }

    private void HandlePausePerformed(InputAction.CallbackContext ctx)
    {

        PausePerformed?.Invoke();
    }
}
