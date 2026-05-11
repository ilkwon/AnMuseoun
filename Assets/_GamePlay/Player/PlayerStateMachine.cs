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
  private int currentLevel = 1; // 플레이어 레벨 (초기값은 1)
  private float currentEXP = 0f; // 현재 경험치

  public int CurrentLevel => currentLevel;
  public float CurrentEXP => currentEXP;
  //---------------------------------------------------------------------------
  public void GainEXP(float amount)
  {
    currentEXP += amount;
    SaveData.Instance.info.currentEXP = currentEXP; // 경험치 저장
    SaveData.Instance.info.currentLevel = currentLevel; // 레벨 저장
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
    Debug.Log($"### 레벨업! 현재 레벨: {currentLevel}, 이동 속도: {controller.MoveSpeed}");
    SaveData.Instance.info.currentLevel = currentLevel; // 레벨 저장
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
    var saveLevel = SaveData.Instance.info.currentLevel;
    var saveExp = SaveData.Instance.info.currentEXP;
    currentLevel = saveLevel > 0 ? saveLevel : 1; // 저장된 레벨이 있으면 불러오고, 없으면 1로 초기화
    currentEXP = saveExp > 0 ? saveExp : 0f; // 저장된 경험치가 있으면 불러오고, 없으면 0으로 초기화
    
    var stat = GameDataManager.Instance.GetPlayerStat(currentLevel); // 저장된 레벨에 해당하는 스탯으로 초기화
    controller.MoveSpeed = stat.spd;
    
    GetComponent<PlayerHP>().SetMaxHP(stat.hp); // HP 초기화
    GetComponent<PlayerExpUI>().UpdateUI(); // EXP UI 업데이트
  }
  //---------------------------------------------------------------------------
  private void Update()
  {
    fsm.Update();
  }

  //---------------------------------------------------------------------------
  private void OnApplicationQuit()
  {
    SaveData.Instance.info.currentEXP = currentEXP;
    SaveData.Instance.info.currentLevel = currentLevel;
    SaveData.Instance.Save();
  }
}