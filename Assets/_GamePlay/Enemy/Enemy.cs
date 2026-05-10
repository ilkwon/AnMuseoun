using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
  [Header("스탯")]
  [SerializeField] private float maxHP = 100f;
  [SerializeField] private float attackDamage = 10f;// 나중에 밸런스 데이터로 교체할 부분

  [Header("타입")]
  [SerializeField] private readonly EnemyType enemyType = EnemyType.Skeleton;
  public EnemyType Type => enemyType;
  public float CurrentHP => currentHP;
  public float AttackDamage => attackDamage;
  public bool IsDead => isDead;
  private float currentHP;
  public Action<Enemy> OnDeath;  // 적이 죽었을 때 호출되는 이벤트

  [Header("사망 이펙트")]
  [SerializeField] private readonly GameObject deathEffectPrefab;

  private Animator animator;
  private Transform hpFill;
  private float hpFillMaxScaleX;
  private float hpFillStartX;  // 초기 로컬 X 위치
  private bool isDead = false;
  private EnemyStatEntity stat; // 적 스탯 데이터

  // 피격 플래시
  [Header("피격 플래시")]
  [SerializeField] private Material flashMaterial;
  [SerializeField] private float flashDuration = 0.1f;
  private MeshRenderer[] meshRenderers;
  private Material[] originalMaterials;

  //---------------------------------------------------------------------------
  void Start()
  {
    animator = GetComponent<Animator>();
    stat = GameDataManager.Instance.GetEnemyStat(enemyType);
    maxHP = stat.hp;
    attackDamage = stat.atk;

    currentHP = maxHP;

    hpFill = transform.Find("Hips/Spine/Chest/Neck/Head/HPBar/HP_Fill");
    if (hpFill != null)
    {
      hpFillMaxScaleX = hpFill.localScale.x;
      hpFillStartX = hpFill.localPosition.x;
    }

    // 피격 플래시 초기화
    meshRenderers = GetComponentsInChildren<MeshRenderer>();
    originalMaterials = new Material[meshRenderers.Length];
    for (int i = 0; i < meshRenderers.Length; i++)
      {
        originalMaterials[i] = new Material(meshRenderers[i].sharedMaterial); // 원래 메터리얼 복사본 저장
        meshRenderers[i].sharedMaterial = originalMaterials[i]; // 인스턴스화된 메터리얼로 교체
      }

  }

  //---------------------------------------------------------------------------
  // 데미지 입는 함수
  public void TakeDamage(float damage)
  {
    if (isDead) return;

    currentHP -= damage;
    currentHP = Mathf.Max(0, currentHP);

    // 피격 플래시 시작
    StartCoroutine(FlashCoroutine());

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
  private System.Collections.IEnumerator FlashCoroutine()
  {
    // 메터리얼을 플래시용으로 교체
    foreach (var mr in meshRenderers)
      mr.sharedMaterial = flashMaterial;

    yield return new WaitForSeconds(flashDuration);

    // 원래 메터리얼로 복구
    for (int i = 0; i < meshRenderers.Length; i++)
      meshRenderers[i].sharedMaterial = originalMaterials[i];
  }

  //---------------------------------------------------------------------------
  private void Die()
  {
    SoundManager.Instance.PlayEnemyDeath();
    ProcessGainEXP();
    isDead = true;
    animator.SetFloat(AnimParam.Speed, 0f);

    if (deathEffectPrefab != null)
    {
      Instantiate(deathEffectPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
    }

    OnDeath?.Invoke(this);
    Destroy(gameObject, 0.3f);

  }

  //---------------------------------------------------------------------------
  private void ProcessGainEXP()
  {
    var player = GameObject.FindWithTag("Player");
    if (player.TryGetComponent<PlayerStateMachine>(out var playerSM))
    {
      playerSM.GainEXP(stat.exp_drop);
    }
  }
  //---------------------------------------------------------------------------
}
