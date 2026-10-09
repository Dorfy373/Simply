using UnityEngine;
using UnityEngine.InputSystem;
using System;

[CreateAssetMenu(fileName = "InputReader", menuName = "Input/InputReader")]
public class InputReader : ScriptableObject
{
    private InputSystem_Actions _inputActions;

    public event Action<Vector2> MoveEvent;

    public event Action SprintPerformed;
    public event Action SprintCanceled;
    public event Action JumpPerformed;
    public event Action JumpCanceled;

    public event Action AttackPerformed;
    public event Action AttackCanceled;
    public event Action<Vector2> ZoomPerformed;
    public event Action ZoomCanceled;

    public event Action InteractPerformed;

    public event Action SelectWeapon1Performed;
    public event Action SelectWeapon2Performed;

    public event Action UsePotion1Performed;
    public event Action UsePotion2Performed;

    private void OnEnable()
    {
        if (_inputActions == null)
        {
            _inputActions = new InputSystem_Actions();

            _inputActions.Player.Move.performed += HandleMovePerformed;
            _inputActions.Player.Move.canceled += HandleMoveCanceled;
            _inputActions.Player.Sprint.performed += HandleSprintPerformed;
            _inputActions.Player.Sprint.canceled += HandleSprintCanceled;
            _inputActions.Player.Jump.performed += HandleJumpPerformed;
            _inputActions.Player.Jump.canceled += HandleJumpCanceled;
            _inputActions.Player.Attack.performed += HandleAttackPerformed;
            _inputActions.Player.Attack.canceled += HandleAttackCanceled;
            _inputActions.Camera.CameraZoom.performed += HandleZoomPerformed;
            _inputActions.Camera.CameraZoom.canceled += HandleZoomCanceled;
            _inputActions.Player.Interact.performed += HandleInteractPerformed;
            _inputActions.Player.SelectWeapon1.performed += HandleSelectWeapon1Performed;
            _inputActions.Player.SelectWeapon2.performed += HandleSelectWeapon2Performed;
            _inputActions.Player.UsePotion1.performed += HandleUsePotion1Performed;
            _inputActions.Player.UsePotion2.performed += HandleUsePotion2Performed;
        }

        _inputActions.Player.Enable();
        _inputActions.Camera.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Player.Disable();
        _inputActions.Camera.Disable();
    }

    // --- Обработчики (именованные методы — вот почему отписка будет работать правильно) ---
    private void HandleMovePerformed(InputAction.CallbackContext ctx) => MoveEvent?.Invoke(ctx.ReadValue<Vector2>());
    private void HandleMoveCanceled(InputAction.CallbackContext ctx) => MoveEvent?.Invoke(Vector2.zero);

    private void HandleSprintPerformed(InputAction.CallbackContext ctx) => SprintPerformed?.Invoke();
    private void HandleSprintCanceled(InputAction.CallbackContext ctx) => SprintCanceled?.Invoke();
    private void HandleJumpPerformed(InputAction.CallbackContext ctx) => JumpPerformed?.Invoke();
    private void HandleJumpCanceled(InputAction.CallbackContext ctx) => JumpCanceled?.Invoke();
    private void HandleAttackPerformed(InputAction.CallbackContext ctx) => AttackPerformed?.Invoke();
    private void HandleAttackCanceled(InputAction.CallbackContext ctx) => AttackCanceled?.Invoke();
    private void HandleZoomPerformed(InputAction.CallbackContext ctx) => ZoomPerformed?.Invoke(ctx.ReadValue<Vector2>());
    private void HandleZoomCanceled(InputAction.CallbackContext ctx) => ZoomCanceled?.Invoke();
    private void HandleInteractPerformed(InputAction.CallbackContext ctx) => InteractPerformed?.Invoke();
    private void HandleSelectWeapon1Performed(InputAction.CallbackContext ctx) => SelectWeapon1Performed?.Invoke();
    private void HandleSelectWeapon2Performed(InputAction.CallbackContext ctx) => SelectWeapon2Performed?.Invoke();
    private void HandleUsePotion1Performed(InputAction.CallbackContext ctx) => UsePotion1Performed?.Invoke();
    private void HandleUsePotion2Performed(InputAction.CallbackContext ctx) => UsePotion2Performed?.Invoke();
}