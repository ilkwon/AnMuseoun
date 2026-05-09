using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset(AssetPath = "_Project/Data/Tables")]
public class GameData : ScriptableObject
{
	public List<PlayerStatEntity> PlayerStat;
	public List<EnemyStatEntity> EnemyStat;
	public List<WeaponStatEntity> WeaponStat;
	public List<WaveStatEntity> WaveStat;
}
