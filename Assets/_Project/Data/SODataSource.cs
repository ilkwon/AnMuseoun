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
  public PlayerStatEntity GetPlayerStat(int level) => _data.PlayerStat.Find(x => x.level == level);
  public EnemyStatEntity GetEnemyStat(EnemyType type) => _data.EnemyStat.Find(x => x.id == (int)type);
  public WeaponStatEntity GetWeaponStat(WeaponType id) => _data.WeaponStat.Find(x => x.id == (int)id);
  public WaveStatEntity GetWaveStat(int wave) => _data.WaveStat.Find(x => x.wave == wave);
  //-----------------------------------------------------------------------------
}
