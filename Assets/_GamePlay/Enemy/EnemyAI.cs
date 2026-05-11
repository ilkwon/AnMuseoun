using System;
using System.Collections;
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
  private bool isKnockback = false; // 넉백 중인지 여부
  //---------------------------------------------------------------------------  
  void Start()
  {
    enemy = GetComponent<Enemy>();
    navAgent = GetComponent<NavMeshAgent>();
    animator = GetComponent<Animator>();

    var stat = GameDataManager.Instance.GetEnemyStat(enemy.Type);
    detectRange = stat.detect_range;
    attackRange = stat.attack_range;
    attackCooldown = stat.attack_cooldown;
    navAgent.speed = stat.spd;

    var playerObj = GameObject.Find("CreepyCuteChar");
    if (playerObj != null)
      player = playerObj.transform;

//    Debug.Log($"{name} isOnNavMesh:{navAgent.isOnNavMesh} | pos:{transform.position}");
  }
  //---------------------------------------------------------------------------
  public void Initilize()
  {
    isKnockback = false;
    lastAttackTime = -Mathf.Infinity;

    navAgent.ResetPath();
  }
  //---------------------------------------------------------------------------  
  void Update()
  {

    // 적이 매 프레임 해야 할 일을 먼저 써본다
    if (isKnockback) return;


    if (player == null)
    {
      Debug.LogWarning($"{name} player를 못 찾음");
      return;
    }

    float distance = Vector3.Distance(transform.position, player.transform.position);
    if (distance <= attackRange)
      TryAttack();
    else if (distance <= detectRange)
      Chase();
    else
      Idle();    
  }
  //---------------------------------------------------------------------------  
  private void Idle()
  {
    navAgent.ResetPath();
    animator.SetFloat(AnimParam.Speed, 0f);
  }
  //---------------------------------------------------------------------------  
  // 추적
  private void Chase()
  {
    navAgent.SetDestination(player.position);
    animator.SetFloat(AnimParam.Speed, 1f);
  }
  //---------------------------------------------------------------------------  
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

    // 공격 범위 내에 플레이어가 있으면 데미지 입히기
    if (Vector3.Distance(transform.position, player.position) <= attackRange)
    {
      var playerHP = player.GetComponent<PlayerHP>();
      if (playerHP != null)
      {
        playerHP.TakeDamage(enemy.AttackDamage);
        // 넉백 효과
        //Vector3 knockbackDir = (player.position - transform.position).normalized;
        //player.GetComponent<PlayerController>().Knockback(knockbackDir, enemy.KnockbackForce);        
      }
    }
  }
  //---------------------------------------------------------------------------  
  public void Knockback(Vector3 attackerPosition, float force)
  {
    if (!navAgent.isOnNavMesh) return;
    StartCoroutine(KnockbackCoroutine(attackerPosition, force));
  }
  //---------------------------------------------------------------------------
  private IEnumerator KnockbackCoroutine(Vector3 attackerPosition, float force)
  {
    isKnockback = true;
    navAgent.ResetPath();

    Vector3 dir = (transform.position - attackerPosition).normalized;
    dir.y = 0f; // 수평 방향으로만 넉백

    // 밀려나는 구간(0.15초, 점점 감소)
    float duration = 0.12f; // 0.15f → 0.12f로 줄임 --- IGNORE ---
    float elapsed = 0f;
    while (elapsed < duration)
    {
      float t = 1f - (elapsed / duration); // 1 → 0
      navAgent.velocity = dir * force * t;
      elapsed += Time.deltaTime;
      yield return null;
    }

    navAgent.velocity = Vector3.zero;
    yield return new WaitForSeconds(0.1f); // 잠깐 멈췄다가 다시 행동 가능

    isKnockback = false;
  }
  //--------------------------------------------------------------------------- 
  void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, detectRange);
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, attackRange);
  }
}