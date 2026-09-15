using UnityEngine;

[DefaultExecutionOrder(100)]
[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public sealed class ChaseEnemyAnimation : MonoBehaviour
{
    private static readonly int IsChasing = Animator.StringToHash("IsChasing");
    private static readonly int Idle = Animator.StringToHash("Base Layer.Idle");

    private ChaseEnemy enemy;
    private Animator animator;

    private void Awake()
    {
        enemy = GetComponentInParent<ChaseEnemy>();
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;
    }

    private void OnEnable()
    {
        animator.SetBool(IsChasing, false);
        animator.Play(Idle, 0, 0f);
    }

    private void Update()
    {
        animator.SetBool(IsChasing, enemy.State == ChaseEnemy.ChaseState.Chase);
    }
}
