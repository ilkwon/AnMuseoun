using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerController))]
public class PlayerStateMachine : MonoBehaviour
{
  
  private StateMachine fsm;
  private Animator animator;
  private PlayerController controller;

  public StateMachine FSM => fsm;
  public Animator Animator => animator;
  public PlayerController Controller => controller;
  //---------------------------------------------------------------------------
  public void Awake()
  {
    fsm = new StateMachine();
    animator = GetComponent<Animator>();
    controller = GetComponent<PlayerController>();
    
    // 상태 등록
    fsm.AddState(new PlayerIdleState(this));
    fsm.AddState(new PlayerMoveState(this));
    fsm.AddState(new PlayerAttackState(this));

    // 초기 상태
    fsm.ChangeState<PlayerIdleState>();
  }

  //---------------------------------------------------------------------------
  private void Update()
  {
    fsm.Update();
  }

  //---------------------------------------------------------------------------
}
