using UnityEngine;

public class GameDataManager : Singleton<GameDataManager>
{
  [SerializeField] private GameData _gameData;
  private IDataSource _source;

  //-----------------------------------------------------------------------------
  protected override void Awake()
  {
    base.Awake();
    
    _source = new SODataSource(_gameData);
  }

  //----------------------------------------------------------------------------- 
  public PlayerStatEntity     GetPlayerStat(int level)        => _source.GetPlayerStat(level);
  public EnemyStatEntity      GetEnemyStat(EnemyType type)    => _source.GetEnemyStat(type);
  public WeaponStatEntity     GetWeaponStat(WeaponType id)    => _source.GetWeaponStat(id);
  public WaveStatEntity       GetWaveStat(int wave)           => _source.GetWaveStat(wave);
  //-----------------------------------------------------------------------------
}