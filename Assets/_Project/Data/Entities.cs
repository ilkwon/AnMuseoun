using UnityEngine;

//-----------------------------------------------------------------------------
[System.Serializable]
public class PlayerStatEntity
{
  public int id;
  public int level;
  public float hp;
  public float atk; // attack
  public float def; // defense
  public float spd; // speed
  public float exp_required;  // 다음 레벨까지 필요한 경험치
  public int version;
}
//-----------------------------------------------------------------------------
[System.Serializable]
public class EnemyStatEntity
{
  public int id;
  public string name;
  public float hp;
  public float atk;
  public float def;
  public float spd;
  public float exp_drop;
  public float detect_range;
  public float attack_range;
  public float attack_cooldown;
  public int version;
  public bool server_validate;
}

//-----------------------------------------------------------------------------
[System.Serializable]
public class WeaponStatEntity
{
  public int id;
  public string name;
  public int weapon_type;
  public float damage_multiplier; //
  public int version;
}

//-----------------------------------------------------------------------------
[System.Serializable]
public class WaveStatEntity
{
  public int id;
  public int wave;
  public int enemy_type;
  public int count;
  public float spawn_interval;
  public float cooldown;
  public int version;
}

//-----------------------------------------------------------------------------