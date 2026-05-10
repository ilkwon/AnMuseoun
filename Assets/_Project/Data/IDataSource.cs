using System.Collections.Generic;
using UnityEngine;

public interface IDataSource
{
  PlayerStatEntity GetPlayerStat(int level);
  EnemyStatEntity GetEnemyStat(EnemyType type);
  WeaponStatEntity GetWeaponStat(WeaponType id);
  WaveStatEntity GetWaveStat(int wave);
  List<WaveStatEntity> GetWaveStatsByWave(int wave);
}
