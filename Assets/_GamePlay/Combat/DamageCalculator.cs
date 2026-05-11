using UnityEngine;

public static class DamageCalculator
{
    
    public static DamageResult Calculate(int playerLevel, WeaponType weaponType, float targetDefense = 0f)
    {
        var gdm = GameDataManager.Instance;
        if (gdm == null)
        {
            Debug.LogError("GameDataManager가 없습니다!");
            return new DamageResult { 
              damage = 10f, 
              isCritical = false, 
              weaponType = weaponType 
            };
        }
    
        var playerStat = gdm.GetPlayerStat(playerLevel);
        var weaponStat = gdm.GetWeaponStat(weaponType);

        float baseDamage  = playerStat.atk * weaponStat.damage_multiplier;
        bool isCritical   = Random.value < GetCriticalChance(playerStat.spd);
        float finalDamage = isCritical ? baseDamage * 2f : baseDamage;
        finalDamage = Mathf.Max(1f, finalDamage - targetDefense); // 방어력 적용 후 최소 1 이상의 피해량 보장

        return new DamageResult
        {
            damage     = finalDamage,
            isCritical = isCritical,
            weaponType = weaponType
        };
    }

    private static float GetCriticalChance(float spd)
    {
        return spd * 0.05f;
    }
}