using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
  [Header("AI")]
  [SerializeField] private float moveSpeed = 3f;
  [SerializeField] private float detectRange = 50f;

  [Header("스탯")]
  [SerializeField] private float maxHP = 100f;

  [Header("타입")]
  [SerializeField] private EnemyType enemyType = EnemyType.Skeleton;
  public EnemyType Type => enemyType;

  private float currentHP;

  [Header("사망 이펙트")]
  [SerializeField] private GameObject deathEffectPrefab;

  private Transform target;
  private Animator animator;
  private Transform hpFill;
  private float hpFillMaxScaleX;
  private float hpFillStartX;  // 초기 로컬 X 위치
  private bool isDead = false;

  //---------------------------------------------------------------------------
  void Start()
  {
    animator = GetComponent<Animator>();
    currentHP = maxHP;

    var player = GameObject.Find("CreepyCuteChar");
    if (player != null)
      target = player.transform;

    hpFill = transform.Find("Hips/Spine/Chest/Neck/Head/HPBar/HP_Fill");
    if (hpFill != null)
    {
      hpFillMaxScaleX = hpFill.localScale.x;
      hpFillStartX = hpFill.localPosition.x;
    }
  }
  //---------------------------------------------------------------------------
  void Update()
  {
    if (isDead || target == null) return;

    float distance = Vector3.Distance(transform.position, target.position);

    if (distance < detectRange)
    {
      Vector3 dir = (target.position - transform.position).normalized;
      dir.y = 0;
      transform.position += dir * moveSpeed * Time.deltaTime;

      Quaternion lookRotation = Quaternion.LookRotation(dir);
      transform.rotation = Quaternion.Slerp(
        transform.rotation, lookRotation, GameConst.RotationSpeed * Time.deltaTime
      );

      animator.SetFloat(AnimParam.Speed, 1f);
    }
    else
    {
      animator.SetFloat(AnimParam.Speed, 0f);
    }
  }
  //---------------------------------------------------------------------------
  public void TakeDamage(float damage)
  {
    if (isDead) return;

    currentHP -= damage;
    currentHP = Mathf.Max(0, currentHP);

    if (hpFill != null)
    {
      float ratio = currentHP / maxHP;

      // 스케일 줄이기
      var s = hpFill.localScale;
      s.x = hpFillMaxScaleX * ratio;
      hpFill.localScale = s;

      // 왼쪽 고정: 줄어든 만큼 오른쪽으로 이동 보정
      var p = hpFill.localPosition;
      p.x = hpFillStartX - (hpFillMaxScaleX * (1f - ratio) * 0.5f);
      hpFill.localPosition = p;
    }

    if (currentHP <= 0)
    {
      Die();
    }
  }
  //---------------------------------------------------------------------------
  private void Die()
  {
    SoundManager.Instance.PlayEnemyDeath();
    isDead = true;
    animator.SetFloat(AnimParam.Speed, 0f);
    
    if (deathEffectPrefab != null)
    {
      Instantiate(deathEffectPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
    }

    Destroy(gameObject, 0.3f);
  }
  //---------------------------------------------------------------------------
}
