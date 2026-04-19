public enum PlayerState
{
    Idle,
    Move,
    Attack,
    Hit,
    Die
}

public enum WeaponType
{
    None = 0,
    Sword = 1,
    Axe = 2
}

public static class AnimParam
{
    public const string Speed      = "Speed";
    public const string Attack     = "Attack";
    public const string Hit        = "Hit";
    public const string Die        = "Die";
    public const string WeaponType = "WeaponType";
}

public static class GameConst
{
    public const float StopDistance   = 0.1f;
    public const float AttackRange   = 2.5f;
    public const float RotationSpeed = 10f;
}