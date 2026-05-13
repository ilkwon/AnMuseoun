using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
  public static EnemyPool Instance { get; private set; }
  private Dictionary<int, Queue<GameObject>> pool = new Dictionary<int, Queue<GameObject>>();
  [SerializeField] private GameObject[] enemyPrefabs;
  [SerializeField] private int poolSize = 20;
  
  //---------------------------------------------------------------------------
  void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      Initialize(this.gameObject.transform);
    }
    else
    {
      Destroy(gameObject);
    }
  }

  //---------------------------------------------------------------------------
  private void Initialize(Transform parent)
  {    
    for (int i=0; i<enemyPrefabs.Length; i++)
    {
      var key = i+1;
      pool[key] = new Queue<GameObject>();
      for (int j=0; j<poolSize; j++)
      {
        var go = Instantiate(enemyPrefabs[i], parent);
        go.SetActive(false);
        pool[key].Enqueue(go);
      }
    }
  }

  //---------------------------------------------------------------------------
  public GameObject GetEnemy(int enemyType, Vector3 position)
  {
    GameObject enemy;
    if (pool.ContainsKey(enemyType) && pool[enemyType].Count > 0)
    {
      var go = pool[enemyType].Dequeue();
      go.transform.position = position;
      go.gameObject.SetActive(true);
      enemy = go;
    }
    else
    {
      Debug.Log($"enemyType: {enemyType}, prefabs.Length: {enemyPrefabs.Length}");
      enemy = Instantiate(enemyPrefabs[enemyType-1], position, Quaternion.identity);
    }
    return enemy;
  }

  //---------------------------------------------------------------------------
  public void ReturnEnemy(GameObject enemyObj)
  {
    var enemy =  enemyObj.GetComponent<Enemy>();
    var key = (int)enemy.Type;
    enemyObj.SetActive(false);
    pool[key].Enqueue(enemyObj);
  }
}
