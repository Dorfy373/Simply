using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    private Animator _animator;
    
    private static readonly int _WalkHash = Animator.StringToHash("Walk");
    private static readonly int _RunHash = Animator.StringToHash("Run");
    private static readonly int _DashHash = Animator.StringToHash("Dash");
    private static readonly int _JumpUpHash = Animator.StringToHash("JumpUp");
    private static readonly int _FallingHash = Animator.StringToHash("Falling");
    private static readonly int _LandHash = Animator.StringToHash("Land");
    private static readonly int _Attack1Hash = Animator.StringToHash("Attack1");
    private static readonly int _Attack2Hash = Animator.StringToHash("Attack2");
    private static readonly int _Attack3Hash = Animator.StringToHash("Attack3");
    private static readonly int _HitHash = Animator.StringToHash("Hit");
    private static readonly int _DeathHash = Animator.StringToHash("Death");


    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void MovementAnimation(bool isMoving, bool isSprinting)
    {
        bool isWalking = isMoving && !isSprinting;
        bool isRunning = isMoving && isSprinting;

        _animator.SetBool(_WalkHash, isWalking);
        _animator.SetBool(_RunHash, isRunning);
    }
    public void TriggerDash()
    {
        _animator.SetTrigger(_DashHash);
    }
    public void TriggerJumpUp()
    {
        _animator.SetTrigger(_JumpUpHash);
    }

    public void TriggerLand()
    {
        _animator.SetTrigger(_LandHash);
    }

    public void TriggerFalling(bool isFalling)
    {
        _animator.SetBool(_FallingHash, isFalling);
    }

    public void TriggerAttack(int comboIndex)
    {
        switch (comboIndex)
        {
            case 1:
                _animator.SetTrigger(_Attack1Hash);
                break;
            case 2:
                _animator.SetTrigger(_Attack2Hash);
                break;
            case 3:
                _animator.SetTrigger(_Attack3Hash);
                break;
        }
    }

    public void TriggerHit()
    {
        _animator.SetTrigger(_HitHash);
    }

    public void TriggerDeath()
    {
        _animator.SetTrigger(_DeathHash);
    }
}
