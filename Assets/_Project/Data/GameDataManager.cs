using System;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : Singleton<GameDataManager>
{
  [SerializeField] private GameData _gameData;
  private IDataSource _source;

  //-----------------------------------------------------------------------------
  protected override void Awake()
  {
    base.Awake();

    if (_gameData == null)
    {
      Debug.LogError("GameDataManager: _gameData가 연결되지 않았습니다! Inspector 확인 필요.");
      return;
    }

    _source = new SODataSource(_gameData);
  }

  //----------------------------------------------------------------------------- 
  public PlayerStatEntity GetPlayerStat(int level)
  {
    return _source.GetPlayerStat(level);
  } 
  public EnemyStatEntity GetEnemyStat(EnemyType type) => _source.GetEnemyStat(type);
  public WeaponStatEntity GetWeaponStat(WeaponType weaponType) => _source.GetWeaponStat(weaponType);
  public WaveStatEntity GetWaveStat(int wave) => _source.GetWaveStat(wave);
  public List<WaveStatEntity> GetWaveStatsByWave(int wave) => _source.GetWaveStatsByWave(wave);
  //-----------------------------------------------------------------------------
}