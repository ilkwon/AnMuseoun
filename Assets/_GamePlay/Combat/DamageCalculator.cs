using UnityEngine;

public static class DamageCalculator
{
    public static DamageResult Calculate(int playerLevel, WeaponType weaponType)
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