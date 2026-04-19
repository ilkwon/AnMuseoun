using UnityEngine;

public class CameraController : MonoBehaviour
{
  [Header("추적 대상")]
  [SerializeField] private Transform target;

  [Header("카메라 설정")]
  [SerializeField] private Vector3 offset;
  [SerializeField] private float smoothSpeed = 5f;

  void Start()
  {
    // 현재 카메라와 타겟 사이 거리를 offset으로 자동 계산
    if (target != null)
    {
      offset = transform.position - target.position;
    }
  }

  void LateUpdate()
  {
    if (target == null) return;

    Vector3 desiredPosition = target.position + offset;
    transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
  }

}
