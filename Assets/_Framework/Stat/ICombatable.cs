/// <summary>
/// 전투 행동 인터페이스.
/// 데미지를 주고받는 모든 엔티티가 구현.
/// 
/// IStatOwner  → 스탯 소유 + 주입
/// ICombatable → 전투 행동 (HP 감소, 공격 등 별도 인터페이스)
/// </summary>
public interface ICombatable
{
  float CurrentHP { get; }
  
  float MaxHp { get; }
  void TakeDamage(float damage);
  bool IsDead { get; }  
}
