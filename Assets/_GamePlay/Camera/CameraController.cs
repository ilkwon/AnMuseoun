using UnityEngine;

public class CameraController : MonoBehaviour
{
  [Header("추적 대상")]
  [SerializeField] private Transform target;

  [Header("카메라 설정")]
  [SerializeField] private Vector3 offset;
  [SerializeField] private float smoothSpeed = 5f;

  // 카메라 흔들림 효과를 위한 변수
  private Vector3 shakeOffset = Vector3.zero;
  //-------------------------------------------------------------------------
  void Start()
  {
    // 현재 카메라와 타겟 사이 거리를 offset으로 자동 계산
    if (target != null)
    {
      offset = transform.position - target.position;
    }
  }
  public void Shake(float duration = 0.12f, float intensity = 0.3f)
  {
    StartCoroutine(ShakeCoroutine(duration, intensity));
  }
  private System.Collections.IEnumerator ShakeCoroutine(float duration, float intensity)
  {
    float elapsed = 0f;
    while (elapsed < duration)
    {
      float t = 1 - (elapsed / duration); // 시간이 지날수록 감소하는 값 (1 -> 0)
      shakeOffset = Random.insideUnitSphere * intensity * t;
      shakeOffset.y = 0f; // 수평 방향으로만 흔들리도록 y축은 고정
      elapsed += Time.deltaTime;
      yield return null;
    }
    shakeOffset = Vector3.zero;
  }
  
  //-------------------------------------------------------------------------
  void LateUpdate()
  {
    if (target == null) return;

    Vector3 desiredPosition = target.position + offset;
    transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime) + shakeOffset;
  }
  //-------------------------------------------------------------------------
}
