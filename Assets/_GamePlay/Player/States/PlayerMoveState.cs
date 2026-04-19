using UnityEngine;

public class PlayerMoveState : IState
{
  private PlayerStateMachine owner;

  public PlayerMoveState(PlayerStateMachine owner)
  {
    this.owner = owner;
  }

  public void Enter()
  {
    owner.Animator.SetFloat(AnimParam.Speed, 1f); // Walk 재생(전환)
  }
  
  public void Update()
  {
    //Debug.Log("MoveState.Update 호출됨");
    owner.Controller.MoveToTarget();
  }

  public void Exit()
  {
    owner.Animator.SetFloat(AnimParam.Speed, 0f); // Idle로 복귀
  }
}
