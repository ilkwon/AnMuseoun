using UnityEngine;
/// <summary>
/// 테이블(SO/SQLite/서버)에서 읽어온 원본 수치를 담는 그릇.
///Enemy든 Player든 Creature든 공통으로 사용.
/// </summary>
public struct StatData
{
  // 기본 전투 스탯
  public float hp;
  public float atk;
  public float def;
  public float spd;

  // 전투 행동
  public float attackRange;
  public float attackCooldown;
  public float detectRange;       // 적 감지 범위 (Enemy 전용)

  // 웨이브 버프 (EnemySpawner가 주입)
  public float buffMultiplier;

  // 보상 (Enemy 전용)
  public int expDrop;
  public int soulDrop;

  // buff 적용 최종값
  public float FinalHp => hp * buffMultiplier;
  public float FinalAtk => atk * buffMultiplier;

  // 기본값 (buffMultiplier 1.0 보장)
  public static StatData Default => new() { buffMultiplier = 1f };
}
