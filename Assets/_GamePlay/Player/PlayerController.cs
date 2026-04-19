using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerStateMachine))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
  [Header("이동")]
  [SerializeField] private float moveSpeed = 5f;
  [Header("이펙트")]
  [SerializeField] private GameObject clickEffectPrefab;

  private Camera mainCamera;
  private PlayerStateMachine playerState;
  private Animator animator;
  private CharacterController characterController;
  private Vector3 targetPosition;
  private bool isMoving = false;

  // 공격 시 회전 관련
  private bool isRotatingToAttack = false;
  private Quaternion attackRotation;
  private float attackRotateSpeed = 20f;
  //---------------------------------------------------------------------------
  private void Start()
  {
    mainCamera = Camera.main;
    animator = GetComponent<Animator>();
    playerState = GetComponent<PlayerStateMachine>();
    characterController = GetComponent<CharacterController>();
    targetPosition = transform.position;
  }
  //---------------------------------------------------------------------------
  private void Update()
  {
    HandleInput();
    HandleAttackRotation();
  }
  //---------------------------------------------------------------------------
  private void HandleInput()
  {
    if (Mouse.current == null) return;

    // 오른쪽 클릭 = 이동
    if (Mouse.current.rightButton.wasPressedThisFrame)
    {
      Vector2 mousePos = Mouse.current.position.ReadValue();
      Ray ray = mainCamera.ScreenPointToRay(mousePos);

      if (Physics.Raycast(ray, out RaycastHit hit))
      {
        targetPosition = hit.point;
        targetPosition.y = transform.position.y;

        if (clickEffectPrefab != null)
          Instantiate(clickEffectPrefab, hit.point + Vector3.up * 0.1f, Quaternion.Euler(-90f, 0f, 0f));

        isRotatingToAttack = false;
        playerState.FSM.ChangeState<PlayerMoveState>();
      }
    }

    // 왼쪽 클릭 = 클릭 방향으로 부드럽게 회전 후 공격
    if (Mouse.current.leftButton.wasPressedThisFrame)
    {
      Vector2 mousePos = Mouse.current.position.ReadValue();
      Ray ray = mainCamera.ScreenPointToRay(mousePos);

      if (Physics.Raycast(ray, out RaycastHit hit))
      {
        Vector3 dir = hit.point - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.01f)
        {
          attackRotation = Quaternion.LookRotation(dir);
          isRotatingToAttack = true;
        }
      }

      playerState.FSM.ChangeState<PlayerAttackState>();
    }
  }
  //---------------------------------------------------------------------------
  private void HandleAttackRotation()
  {
    if (!isRotatingToAttack) return;

    transform.rotation = Quaternion.Slerp(
      transform.rotation, attackRotation, attackRotateSpeed * Time.deltaTime
    );

    if (Quaternion.Angle(transform.rotation, attackRotation) < 1f)
    {
      transform.rotation = attackRotation;
      isRotatingToAttack = false;
    }
  }
  //---------------------------------------------------------------------------
  public void MoveToTarget()
  {
    float distance = Vector3.Distance(transform.position, targetPosition);

    if (distance > GameConst.StopDistance)
    {
      Vector3 dir = (targetPosition - transform.position).normalized;

      // 중력적용
      if (!characterController.isGrounded)
      {
        dir += Physics.gravity * Time.deltaTime;
      } else
      {
        dir.y = 0f; // 수평 이동만
      }

      // CharacterController.Move() — 벽에 부딪히면 자동으로 막힘
      characterController.Move(dir * moveSpeed * Time.deltaTime);

      Quaternion lookRotation = Quaternion.LookRotation(dir);
      transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation,
        GameConst.RotationSpeed * Time.deltaTime);
    }
    else
    {
      isMoving = false;
      playerState.FSM.ChangeState<PlayerIdleState>();
    }
  }
  //---------------------------------------------------------------------------
}
