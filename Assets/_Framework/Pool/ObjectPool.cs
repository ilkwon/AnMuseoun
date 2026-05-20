using UnityEngine;
using System.Collections.Generic;

public class ObjectPool<T> where T : MonoBehaviour
{
  private Queue<T> _pool;
  private T _prefab;
  private Transform _parent;
  //----------------------------------------------------------------------------
  public ObjectPool(T prefab, int prewarm, Transform parent)
  {
    _prefab = prefab;
    _parent = parent;
    _pool = new Queue<T>();

    for (int i=0; i<prewarm; i++){
      var obj = Create();
      _pool.Enqueue(obj);
    }
  }
  //----------------------------------------------------------------------------
  T Create()
  {
    T obj = GameObject.Instantiate<T>(_prefab, _parent);
      SetActive(obj, false);

      return obj;
  }
  //----------------------------------------------------------------------------
  public T Get()
  {   
    T obj = _pool.Count > 0 ? _pool.Dequeue() : Create();
      SetActive(obj, true);
      
      return obj;
  }
  
  //----------------------------------------------------------------------------
  private void SetActive(T obj, bool active)
  {
    if (obj.gameObject.activeSelf != active)
      obj.gameObject.SetActive(active);
  }
  //----------------------------------------------------------------------------
  public void Return(T obj)
  {
    SetActive(obj, false);
    
    _pool.Enqueue(obj);
  }

  //----------------------------------------------------------------------------
}
