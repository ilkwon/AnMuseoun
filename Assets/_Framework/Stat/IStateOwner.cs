/// <summary>
/// 스탯 소유 인터페이스.
/// Enemy, Player, Creature 등 스탯을 가진 모든 엔티티가 구현.
///
/// IStatOwner  → 스탯 소유 + 주입
/// ICombatable → 전투 행동 (별도 인터페이스)
/// </summary>
public interface IStatOwner
{
    /// <summary>현재 스탯 (읽기 전용)</summary>
    StatData Stats { get; }

    /// <summary>
    /// 외부에서 스탯 주입 (Push 방식).
    /// Pool에서 꺼낼 때 / 스폰 시 호출.
    /// </summary>
    void Setup(StatData statData);
}