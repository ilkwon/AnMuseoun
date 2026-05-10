
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using SceneManagement = UnityEngine.SceneManagement;
public class EnemySpawner : MonoBehaviour
{
  [Header("스폰 포인트")]
  [SerializeField] private Transform[] spawnPoints;

  // 밸런스 데이터 — 나중에 DataManager로 교체할 부분
  [Header("웨이브 데이터")]
  
  [SerializeField] private float spawnRadius = 25f;
  [SerializeField] private GameObject stageClearEffectPrefab;

  // 프리팹 — 나중에 Addressable/AssetBundle로 교체 가능
  [Header("프리팹")]
  [SerializeField] private GameObject[] enemyPrefabs;

  private int currentWave = 0;
  private bool isSpawning = false;
  private bool doWaving;
  private int alivingEnemyCount;

  private void Start()
  {
    currentWave = SaveData.Instance.info.currentWave;
    StartCoroutine(StartNextWave());
    doWaving = true;
  }
  //---------------------------------------------------------------------------
  private void Update()
  {
    if (!doWaving) return; ;
    if (isSpawning) return;
    if (alivingEnemyCount > 0) return;
    
    StartCoroutine(StartNextWave());
    Debug.Log($"웨이브 {currentWave} 완료  남은 적 수: {alivingEnemyCount}");
    SaveCurrentProgress();
  }

  //---------------------------------------------------------------------------
  private IEnumerator StartNextWave(float waitTime = 2f)
  {
    isSpawning = true;

    yield return new WaitForSeconds(waitTime);    
    var waveStats = GameDataManager.Instance.GetWaveStatsByWave(currentWave + 1);
    if (waveStats == null || waveStats.Count == 0) 
      waveStats = GameDataManager.Instance.GetWaveStatsByWave(100); // 웨이브 데이터가 없으면 100번 웨이브 데이터로 대체 (마지막 웨이브 반복)
    foreach (var waveStat in waveStats)
    {     
      for (int i = 0; i < waveStat.count; i++)
      {
        SpawnEnemy(waveStat.enemy_type);
        yield return new WaitForSeconds(waveStat.spawn_interval); // 스폰 간격 조절      
      }
    }
    currentWave++;
    isSpawning = false;    
  }

  //---------------------------------------------------------------------------
  private void SpawnEnemy(int enemyType)
  {
    if (enemyType < 1 || enemyType - 1 >= enemyPrefabs.Length) return;    

    int spawnIndex = Random.Range(0, spawnPoints.Length);
    Vector3 spawnPosition = 
      spawnPoints[spawnIndex].position + Random.insideUnitSphere * spawnRadius;
    spawnPosition.y = 0; // 지면과 맞추기

    var prefab = enemyPrefabs[enemyType - 1];    
    var spawnObj = Instantiate(prefab, spawnPosition, Quaternion.identity);
    alivingEnemyCount++;
    //Debug.Log($"적 스폰! 현재 생존 적 수: {alivingEnemyCount}");

    var enemy = spawnObj.GetComponent<Enemy>();
    if (enemy != null)    {
      enemy.OnDeath += HandleEnemyDeath;
    }
  }
  //---------------------------------------------------------------------------
  private void HandleEnemyDeath(Enemy enemy)
  {
    alivingEnemyCount--;
    //Debug.Log($"적 사망! 현재 생존 적 수: {alivingEnemyCount}");    
    enemy.OnDeath -= HandleEnemyDeath;
  }

  //---------------------------------------------------------------------------
  private void OnApplicationQuit()
  {
    SaveCurrentProgress();
  }
  // 게임 종료 시 현재 웨이브 저장
  private void SaveCurrentProgress()
  {
    SaveData.Instance.info.currentWave = currentWave;
    SaveData.Instance.Save();
    Debug.Log($"게임 종료 - 현재 웨이브 {currentWave} 저장 완료");
  }
}