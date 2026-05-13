using UnityEngine;

public class PlayerAttackState : IState
{
  private readonly PlayerStateMachine owner; // 상태를 소유한 플레이어 FSM
  private bool hasDealtDamage = false;
  private TrailRenderer attackTrail;
  private ParticleSystem handFire;

  // 번개 프리팹 (Resources 폴더 없이 직접 참조)
  private static GameObject lightningPrefab;

  //---------------------------------------------------------------------------
  public PlayerAttackState(PlayerStateMachine owner)
  {
    this.owner = owner;
    string trail =
        "Hips/Spine/Chest/LeftShoulder/LeftUpperArm/LeftLowerArm/LeftHand/WeaponHolder/BattleAxe/HeadCenter/AttackTrail";
    var trailObj = owner.transform.Find(trail);

    Debug.Log($"AttackTrail 찾음: {trailObj != null}");  // ← 추가

    if (trailObj != null)
    {
      attackTrail = trailObj.GetComponent<TrailRenderer>();
      Debug.Log($"TrailRenderer: {attackTrail != null}");  // ← 추가
    }
  }

  //---------------------------------------------------------------------------
  public void Enter()
  {
    owner.Animator.SetFloat(AnimParam.Speed, 0f);
    owner.Animator.SetTrigger(AnimParam.Attack);
    hasDealtDamage = false;

    if (attackTrail != null)
    {
      attackTrail.Clear();
      attackTrail.emitting = false;
    }
    if (handFire != null) handFire.Play();
  }

  //---------------------------------------------------------------------------
  public void Update()
  {
    if (!hasDealtDamage)
    {
      var stateInfo = owner.Animator.GetCurrentAnimatorStateInfo(0);

      // 공격 모션 40% 지점 → 데미지 + 번개 발사
      if (stateInfo.normalizedTime >= 0.4f)
      {
        DealDamage();
        SpawnLightning();
        hasDealtDamage = true;
      }
    }
  }

  //---------------------------------------------------------------------------
  public void Exit()
  {
    if (attackTrail != null) attackTrail.emitting = false;
    if (handFire != null) handFire.Stop();
  }

  //---------------------------------------------------------------------------
  private void DealDamage()
  {
    bool anyCritical = false;
    var hits = Physics.OverlapSphere(
      owner.transform.position,
      GameConst.AttackRange);

    foreach (var hit in hits)
    {
      var enemy = hit.GetComponent<Enemy>();
      if (enemy != null)
      {
        var result = DamageCalculator.Calculate(owner.Stats, WeaponType.Axe, enemy.Def);
        if (result.isCritical)
          anyCritical = true;

        enemy.TakeDamage(result.damage);
        //Debug.Log($"Dealt {result.damage} damage to {enemy.name} (Critical: {result.isCritical})"); 
        var ai = enemy.GetComponent<EnemyAI>();
        if (ai != null)
          ai.Knockback(owner.transform.position, 8f);
      }
    }

    if (anyCritical)
    {
      if (attackTrail != null)
      {
        attackTrail.Clear();
        //attackTrail.startColor = Color.yellow;
        //attackTrail.endColor = Color.red;
        attackTrail.emitting = true;
      }
      Camera.main.GetComponent<CameraController>().Shake(0.12f, 0.5f);
    }

    SoundManager.Instance.PlayHit();
  }

  //---------------------------------------------------------------------------
  private void SpawnLightning()
  {
    // 프리팹 로딩 (최초 1회만)
    if (lightningPrefab == null)
    {
#if UNITY_EDITOR
      lightningPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(
        "Assets/_Project/Prefabs/LightningBolt.prefab"
      );
#endif
    }

    if (lightningPrefab == null) return;

    // 플레이어 앞쪽에서 바라보는 방향으로 발사
    var spawnPos = owner.transform.position + Vector3.up * 3f + owner.transform.forward * 1f;
    var rotation = Quaternion.LookRotation(owner.transform.forward);

    //  Object.Instantiate(lightningPrefab, spawnPos, rotation);
  }

  //---------------------------------------------------------------------------
}
