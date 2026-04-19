using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
  [Header("AI")]
  [SerializeField] private float detectRange = 50f;
  [SerializeField] private float attackRange = 2.5f;
  [SerializeField] private float attackCooldown = 2f;
  private Enemy enemy;
  private NavMeshAgent navAgent;
  private Animator animator;
  private Transform player;
  private float lastAttackTime = -Mathf.Infinity;

  void Start()
  {
    enemy = GetComponent<Enemy>();
    navAgent = GetComponent<NavMeshAgent>();
    animator = GetComponent<Animator>();

    var playerObj = GameObject.Find("CreepyCuteChar");
    if (playerObj != null)
      player = playerObj.transform;
  }
  void Update()
  {
    // 적이 매 프레임 해야 할 일을 먼저 써본다
    // 빨간줄은 나중에 Cmd+. 로 채운다
    if (enemy.IsDead) return;

    float distance = Vector3.Distance(transform.position, player.transform.position);
    if (distance <= attackRange)
      TryAttack();
    else if (distance <= detectRange)
      Chase();
    else
      Idle();
  }

  private void Idle()
  {
    navAgent.ResetPath();
    animator.SetFloat(AnimParam.Speed, 0f);
  }

  // 추적
  private void Chase()
  {
    navAgent.SetDestination(player.position);
    animator.SetFloat(AnimParam.Speed, 1f);
  }

  private void TryAttack()
  {
    // 공격 범위 안에 들어왔으면 NavMeshAgent 멈추고 공격 애니메이션
    navAgent.ResetPath();
    animator.SetFloat(AnimParam.Speed, 0f);

    // 플레이어를 바라보기.
    Vector3 dir = (player.position - transform.position).normalized;
    dir.y = 0f; // 수평 방향으로만 회전
    transform.rotation = Quaternion.LookRotation(dir);

    // 풀다운 체크
    if (Time.time - lastAttackTime < attackCooldown) return;
    lastAttackTime = Time.time;

    // 공격 트리거
    animator.SetTrigger(AnimParam.Attack);
  }
}