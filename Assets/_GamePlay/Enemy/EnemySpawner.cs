
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
  [SerializeField] private int[] waveEnemyCounts = { 3 };
  [SerializeField] private float waveCooldown = 3f;
  [SerializeField] private float spawnRadius = 25f;
  [SerializeField] private GameObject stageClearEffectPrefab;

  // 프리팹 — 나중에 Addressable/AssetBundle로 교체 가능
  [Header("프리팹")]
  [SerializeField] private GameObject enemyPrefab;

  private int currentWave = 0;
  private bool isSpawning = false;
  private bool doWaving;
  private int alivingEnemyCount;

  private void Start()
  {
    StartCoroutine(StartNextWave());
    doWaving = true;
  }

  private void Update()
  {
    if (!doWaving) return; ;
    if (isSpawning) return;

    if (alivingEnemyCount == 0)
    {
      if (isLastWave())
      {
        StartCoroutine(StageClear(3f));
        Debug.Log("모든 웨이브 완료!");
      }
      else
      {
        StartCoroutine(StartNextWave(waveCooldown));
      }
    }
  }

  private bool isLastWave()
  {
    return currentWave >= waveEnemyCounts.Length - 1;
  }

  private IEnumerator StageClear(float waitTime = 1.6f)
  {
    doWaving = false;
    Debug.Log("스테이지 클리어!");
    if (stageClearEffectPrefab != null)
    {
      var player = GameObject.Find("CreepyCuteChar");
      Instantiate(stageClearEffectPrefab, player.transform.position, stageClearEffectPrefab.transform.rotation);
      Debug.Log("###### 스테이지 클리어 이펙트 생성! #######");
    }
    yield return new WaitForSeconds(waitTime);
    //SceneManagement.SceneManager.LoadScene("MainMenu");
    Debug.Log("메인 메뉴로 돌아가기");
    
  }
  //---------------------------------------------------------------------------
  private IEnumerator StartNextWave(float waitTime = 2f)
  {
    isSpawning = true;

    yield return new WaitForSeconds(waitTime);    
    int enemyCount = waveEnemyCounts[currentWave];
    for (int i = 0; i < enemyCount; i++)
    {
      SpawnEnemy();
      yield return new WaitForSeconds(0.5f); // 스폰 간격 조절      
    }
    isSpawning = false;
    doWaving = true;
    currentWave++;
  }

  //---------------------------------------------------------------------------
  private void SpawnEnemy()
  {
    int spawnIndex = Random.Range(0, spawnPoints.Length);
    Vector3 spawnPosition = spawnPoints[spawnIndex].position + Random.insideUnitSphere * spawnRadius;
    spawnPosition.y = 0; // 지면에 고정
    var spawnObj = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
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
    Debug.Log($"적 사망! 현재 생존 적 수: {alivingEnemyCount}");    
    enemy.OnDeath -= HandleEnemyDeath;
  }

  //---------------------------------------------------------------------------
}