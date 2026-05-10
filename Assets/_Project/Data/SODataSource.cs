using System.Collections.Generic;
using UnityEngine;

public class SODataSource : IDataSource
{
  private GameData _data;
  //-----------------------------------------------------------------------------
  public SODataSource(GameData data)
  {
    _data = data;
  }

  //-----------------------------------------------------------------------------
  public PlayerStatEntity GetPlayerStat(int level) 
    => _data.PlayerStat.Find(x => x.level == level);
  public EnemyStatEntity GetEnemyStat(EnemyType type) 
    => _data.EnemyStat.Find(x => x.id == (int)type);
  public WeaponStatEntity GetWeaponStat(WeaponType weaponType) 
    => _data.WeaponStat.Find(x => x.weapon_type == (int)weaponType);
  public WaveStatEntity GetWaveStat(int wave) 
    => _data.WaveStat.Find(x => x.wave == wave);
  public List<WaveStatEntity> GetWaveStatsByWave(int wave) 
    => _data.WaveStat.FindAll(x => x.wave == wave);  
  //-----------------------------------------------------------------------------
}
