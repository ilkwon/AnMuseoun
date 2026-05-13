using UnityEngine;

public static class DamageCalculator
{

  public static DamageResult Calculate(StatData attackerStat, WeaponType weaponType, float targetDefense = 0f)
  {

    if (GameDataManager.Instance == null)
    {
      Debug.LogError("GameDataManager가 없습니다!");
      return new DamageResult
      {
        damage = 10f,
        isCritical = false,
        weaponType = weaponType
      };
    }

    var weaponStat = GameDataManager.Instance.GetWeaponStat(weaponType);
    float baseDamage = attackerStat.atk * weaponStat.damage_multiplier;
    bool isCritical = Random.value < GetCriticalChance(attackerStat.spd);
    float finalDamage = isCritical ? baseDamage * 2f : baseDamage;
    finalDamage = Mathf.Max(1f, finalDamage - targetDefense); // 방어력 적용 후 최소 1 이상의 피해량 보장

    return new DamageResult
    {
      damage = finalDamage,
      isCritical = isCritical,
      weaponType = weaponType
    };
  }

  private static float GetCriticalChance(float spd)
  {
    return spd * 0.05f;
  }
}