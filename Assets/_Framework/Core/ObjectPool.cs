using System.Collections;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
  Queue _pool = new Queue();
  private T obj;

  public T Get()
  {    
    return null;
  }

  public void Return(T obj)
  {
    
  }
}
