using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    private Animator _animator;

    private static readonly int _WalkHash = Animator.StringToHash("isWalking");
    private static readonly int _RunHash = Animator.StringToHash("isRunning");
    private static readonly int _AttackHash = Animator.StringToHash("Attack");
    private static readonly int _HitHash = Animator.StringToHash("Hit");
    private static readonly int _DeathHash = Animator.StringToHash("Death");


    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void MovementAnimation(bool isWalking, bool isRunning)
    {
        _animator.SetBool(_WalkHash, isWalking);
        _animator.SetBool(_RunHash, isRunning);
    }

    public void TriggerAttack()
    {
        _animator.SetTrigger(_AttackHash);
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
