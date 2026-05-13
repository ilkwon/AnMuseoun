/// <summary>
/// 게임 이벤트 정의.
/// EventBus로 발행/구독하는 이벤트 데이터 구조체 모음.
/// </summary>

/// <summary>웨이브 변경</summary>
public struct OnWaveChanged
{
    public int wave;
}

/// <summary>플레이어 HP 변경</summary>
public struct OnHPChanged
{
    public float currentHp;
    public float maxHp;
}

/// <summary>게임 오버</summary>
public struct OnGameOver { }

/// <summary>레벨업</summary>
public struct OnLevelUp
{
    public int level;
    public float currentExp;
    public float expToNextLevel;
}