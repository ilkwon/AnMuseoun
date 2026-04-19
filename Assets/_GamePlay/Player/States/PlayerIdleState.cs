using UnityEngine;

public class PlayerIdleState : IState
{
   private PlayerStateMachine owner;

   public PlayerIdleState(PlayerStateMachine owner)
   {
    this.owner = owner;
   }

   public void Enter()
  {
    owner.Animator.SetFloat(AnimParam.Speed, 0f);
  }

  public void Update()
  {
    
  }

  public void Exit()
  {
    
  }
  
}
