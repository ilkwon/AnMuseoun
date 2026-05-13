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

private StatData _stats;
public StatData Stats => _stats;

  private int currentLevel = 1; // 플레이어 레벨 (초기값은 1)
  private float currentEXP = 0f; // 현재 경험치

  public int CurrentLevel => currentLevel;
  public float CurrentEXP => currentEXP;
  //---------------------------------------------------------------------------
  public void GainEXP(float amount)
  {
    currentEXP += amount;

    var stat = GameDataManager.Instance.GetPlayerStat(currentLevel);
    PlayerExpUI expUI = GetComponent<PlayerExpUI>();
    if (expUI != null)
      expUI.UpdateUI();

    if (currentEXP >= stat.exp_required)
    {
      currentEXP -= stat.exp_required;
      LevelUp();
    }
//    Debug.Log($"경험치 획득: {amount}, 현재 EXP: {currentEXP}/{stat.exp_required}");
  }
  //---------------------------------------------------------------------------
  private void LevelUp()
  {
    currentLevel++;
    var stat = GameDataManager.Instance.GetPlayerStat(currentLevel);
    controller.MoveSpeed = stat.spd; // 레벨업 시 스피드 증가
    _stats = new StatData
    {
      hp = stat.hp,
      atk = stat.atk,
      def = stat.def,
      spd = stat.spd
    };
    
    EventBus.Emit(new OnLevelUp 
    {
      level = currentLevel, 
      currentExp = currentEXP, 
      expToNextLevel = stat.exp_required 
    });
    Debug.Log($"### 레벨업! 현재 레벨: {currentLevel}, 이동 속도: {controller.MoveSpeed}");    
  }

  //---------------------------------------------------------------------------
  private void Awake()
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
  private void Start()
  {

    currentLevel = 1;
    currentEXP = 0;
    
    var stat = GameDataManager.Instance.GetPlayerStat(currentLevel); // 저장된 레벨에 해당하는 스탯으로 초기화
    _stats = new StatData
    {
      hp = stat.hp,
      atk = stat.atk,
      def = stat.def,
      spd = stat.spd
    };
    controller.MoveSpeed = stat.spd;
    
    GetComponent<PlayerHP>().SetMaxHP(stat.hp); // HP 초기화
    GetComponent<PlayerExpUI>().UpdateUI(); // EXP UI 업데이트
  }
  //---------------------------------------------------------------------------
  private void Update()
  {
    if (fsm.CurrentState != null)
      fsm.Update();
  }


}