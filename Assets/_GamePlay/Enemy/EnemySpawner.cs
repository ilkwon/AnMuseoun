using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using System.Collections.Generic;

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
  //private Queue<Enemy> _enemyPool = new();
  private Dictionary<int, Queue<Enemy>> _enemyPool = new();
  private int currentWave = 0;
  private bool isSpawning = false;
  private bool doWaving;          // 웨이브 진행 여부 제어
  private int alivingEnemyCount;  // 현재 생존 중인 적 수
  //----------------------------------------------------------------------------
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
    SaveCurrentProgress();
  }

  //---------------------------------------------------------------------------
  // 다음 웨이브 시작 코루틴
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
    //GetComponent<WaveUI>()?.UpdateUI(currentWave);
    EventBus.Emit(new OnWaveChanged { wave = currentWave });
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

    Enemy enemy;
    
    // 풀링된 적이 있으면 재사용, 없으면 새로 생성
    if (_enemyPool.ContainsKey(enemyType) && _enemyPool[enemyType].Count > 0)
    {
      enemy = _enemyPool[enemyType].Dequeue();
      enemy.transform.position = spawnPosition;
      enemy.gameObject.SetActive(true);
      enemy.Initialize();
      enemy.GetComponent<EnemyAI>().Initilize();          
    }
    else
    {
      var prefab = enemyPrefabs[enemyType - 1];
      var spawnObj = Instantiate(prefab, spawnPosition, Quaternion.identity);
      enemy = spawnObj.GetComponent<Enemy>();
    }

    if (enemy != null)
    {
      // StatData 생성 + buff 적용 push
      var baseStat = GameDataManager.Instance.GetEnemyStat(enemy.Type);
      var statData = new StatData
      {
        hp              = baseStat.hp,
        atk             = baseStat.atk,
        def             = baseStat.def,
        spd             = baseStat.spd,
        detectRange     = baseStat.detect_range,
        attackRange     = baseStat.attack_range,
        attackCooldown  = baseStat.attack_cooldown,
        expDrop         =  baseStat.exp_drop,
        buffMultiplier  = 1f // 웨이브 버프는 나중에 GameDataManager에서 가져와서 곱해주는 방식으로 적용할 예정    
      };

      enemy.Setup(statData);
      enemy.GetComponent<EnemyAI>().Setup(statData);
      
      enemy.OnDeath += HandleEnemyDeath;
      alivingEnemyCount++;     
    }
  }
  //---------------------------------------------------------------------------
  private void HandleEnemyDeath(Enemy enemy)
  {
    alivingEnemyCount--;

    //Debug.Log($"적 사망! 현재 생존 적 수: {alivingEnemyCount}");    
    
    enemy.OnDeath -= HandleEnemyDeath;

    enemy.gameObject.SetActive(false);
    var key = ((int)enemy.Type);
    if (!_enemyPool.ContainsKey(key))
      _enemyPool[key] = new Queue<Enemy>();
    _enemyPool[key].Enqueue(enemy);
  }

  //---------------------------------------------------------------------------
  private void OnApplicationQuit()
  {
    SaveCurrentProgress();
  }
  //---------------------------------------------------------------------------
  // 게임 종료 시 현재 웨이브 저장
  private void SaveCurrentProgress()
  {
    SaveData.Instance.info.currentWave = currentWave;
    SaveData.Instance.Save();
    Debug.Log($"게임 종료 - 현재 웨이브 {currentWave} 저장 완료");
  }
}