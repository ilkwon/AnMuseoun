using UnityEngine;

public class ClickEffect : MonoBehaviour
{
  [SerializeField] private float lifetime = 0.5f;

  void Start()
  {
    Destroy(gameObject, lifetime);
  }
}
