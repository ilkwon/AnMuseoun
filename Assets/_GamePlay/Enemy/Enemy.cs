using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour, ICombatable, IStatOwner
{
  [Header("타입")]
  [SerializeField] private EnemyType enemyType = EnemyType.Skeleton;

  [Header("사망 이펙트")]
  [SerializeField] private GameObject deathEffectPrefab;

  public EnemyType Type => enemyType;
  public float MaxHp => _stats.FinalHp;
  public float CurrentHp => _currentHP;
  public float AttackDamage => _stats.atk;
  public bool IsDead => isDead;
  public Action<Enemy> OnDeath;  // 적이 죽었을 때 호출되는 이벤트
  private float _currentHP;
  // 컴포넌트 캐싱
  private Animator animator;
  private Transform hpFill;
  private float hpFillMaxScaleX;
  private float hpFillStartX;  // 초기 로컬 X 위치
  private bool isDead = false;

  // 피격 플래시
  [Header("피격 플래시")]
  [SerializeField] private Material flashMaterial;
  [SerializeField] private float flashDuration = 0.1f;
  private MeshRenderer[] meshRenderers;
  private Material[] originalMaterials;
  private StatData _stats;
  public float Def => _stats.def; // 방어력 추가

  public StatData Stats => _stats;

  //---------------------------------------------------------------------------
  // 
  void Reset()
  {
    _currentHP = _stats.FinalHp;
    if (meshRenderers != null && originalMaterials != null)
    {
      // 피격 플래시 원래대로 복원.
      for (int i = 0; i < meshRenderers.Length; i++)
      {
        meshRenderers[i].sharedMaterial = originalMaterials[i];
      }
    }
    // HP UI 초기화
    if (hpFill != null)
    {
      var scale = hpFill.localScale;
      scale.x = hpFillMaxScaleX;
      hpFill.localScale = scale;

      var pos = hpFill.localPosition;
      pos.x = hpFillStartX;
      hpFill.localPosition = pos;
    }
  }
  //---------------------------------------------------------------------------
  void Start()
  {
    animator = GetComponent<Animator>();

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
  public void Setup(StatData statData)
  {
    _stats = statData;
    _currentHP = _stats.FinalHp;

    Reset();
  }
  //---------------------------------------------------------------------------
  // 데미지 입는 함수
  public void TakeDamage(float damage)
  {
    if (isDead) return;

    _currentHP -= damage;
    _currentHP = Mathf.Max(0, _currentHP);

    // 피격 플래시 시작
    StartCoroutine(FlashCoroutine());

    UpdateUI_HP();

    if (_currentHP <= 0)
    {
      Die();
    }
  }

  //---------------------------------------------------------------------------
  private void UpdateUI_HP()
  {
    if (hpFill != null)
    {
      if (_stats.FinalHp <= 0) return;
      float ratio = _currentHP / _stats.FinalHp;

      // 스케일 줄이기
      var s = hpFill.localScale;
      s.x = hpFillMaxScaleX * ratio;
      hpFill.localScale = s;

      // 왼쪽 고정: 줄어든 만큼 오른쪽으로 이동 보정
      var p = hpFill.localPosition;
      p.x = hpFillStartX - (hpFillMaxScaleX * (1f - ratio) * 0.5f);
      hpFill.localPosition = p;
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
    //Destroy(gameObject, 0.3f);

  }

  //---------------------------------------------------------------------------
  private void ProcessGainEXP()
  {
    var player = GameObject.FindWithTag("Player");
    if (player.TryGetComponent<PlayerStateMachine>(out var playerSM))
    {
      playerSM.GainEXP(_stats.expDrop);
    }
  }
  //---------------------------------------------------------------------------
  public void Initialize()
  {
    isDead = false;
    OnDeath = null;

    Reset();
  }
  //---------------------------------------------------------------------------
}
