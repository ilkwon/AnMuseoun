using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : Singleton<EnemyPool>
{
  
  private Dictionary<int, ObjectPool<Enemy>> _pool = new();
  [SerializeField] private GameObject[] enemyPrefabs;
  [SerializeField] private int poolSize = 20;
  
  //---------------------------------------------------------------------------
  protected override void Awake()
  {
    base.Awake(); 
    
    Initialize(this.transform);   
  }

  //---------------------------------------------------------------------------
  private void Initialize(Transform parent)
  {
    for (int i=0; i<enemyPrefabs.Length; i++)
    {
      var key = i+1;
      var prefab = enemyPrefabs[i].GetComponent<Enemy>();
      _pool[key] = new ObjectPool<Enemy>(prefab, poolSize, transform);
    }
  }

  //---------------------------------------------------------------------------
  public Enemy GetEnemy(int enemyType, Vector3 position)
  {
    var enemy = _pool[enemyType].Get();
    enemy.gameObject.transform.position = position;
    return enemy;
  }

  //---------------------------------------------------------------------------
  public void ReturnEnemy(Enemy enemy)
  {
    var key = (int)enemy.Type;
    _pool[key].Return(enemy);
  }
}
