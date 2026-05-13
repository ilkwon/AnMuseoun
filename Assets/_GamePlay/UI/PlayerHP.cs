using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHP : MonoBehaviour, ICombatable
{
  [Header("HP")]
  [SerializeField] private float maxHP = 100f;

  // HP UI
  private UnityEngine.UI.Image hpFill;
  private TMP_Text hpText;

  private float currentHP;
  public float MaxHp => maxHP;
  public float CurrentHp => currentHP;
  
  public bool IsDead => currentHP <= 0f;

  //---------------------------------------------------------------------------
  void Start()
  {
    // 흑백 처리 초기화 (씬 리로드 시 원래 색상으로)
    var volume = FindAnyObjectByType<UnityEngine.Rendering.Volume>();
    if (volume != null && volume.profile.TryGet<UnityEngine.Rendering.Universal.ColorAdjustments>(out var ca))
      ca.saturation.Override(0f);
    
    currentHP = maxHP;

    var fill = GameObject.Find("UIRoot/PanelPlayStatus/HpBar/HP_Fill");
    hpFill = fill.GetComponent<UnityEngine.UI.Image>();

    var text = GameObject.Find("UIRoot/PanelPlayStatus/HpBar/HP_Text");
    hpText = text.GetComponent<TMP_Text>();

    UpdateHPUI();
  }
  //---------------------------------------------------------------------------
  private void UpdateHPUI()
  {
    float ratio = currentHP / maxHP;  // 0~1 사이 비율
    if (hpFill != null)
      hpFill.rectTransform.anchorMax = new Vector2(ratio, 1f);

    if (hpText != null)
      hpText.text = $"{Mathf.CeilToInt(currentHP)}/{Mathf.CeilToInt(maxHP)}";
  }

  //---------------------------------------------------------------------------
  public void TakeDamage(float damage)
  {
    if (IsDead) return;

    currentHP -= damage;
    UpdateHPUI();

    EventBus.Emit(new OnHPChanged { currentHp = currentHP, maxHp = maxHP });

    if (currentHP <= 0f)
      Die();

  }

  //---------------------------------------------------------------------------
  private void Die()
  {

    EventBus.Emit(new OnGameOver());

    HardcoreReset();
    GrayscaleScreen();
    ShowGameOverUI();

    // 플레이어 캐릭터 비활성화 (카메라가 따라가야 하니까 Destroy 대신)
    var playerModel = transform.Find("Hips");
    if (playerModel != null)
      playerModel.gameObject.SetActive(false);
  }
  //---------------------------------------------------------------------------
  private void HardcoreReset()
  {
    SaveData.Instance.info.currentLevel = 1;
    SaveData.Instance.info.currentEXP = 0f;
    SaveData.Instance.info.currentWave = 0;
    SaveData.Instance.info.enemyBuffLevel = 0;

    SaveData.Instance.Save();
  }

  //---------------------------------------------------------------------------
  private void ShowGameOverUI()
  {
    var textObj = GameObject.Find("UIRoot/PanelPlayStatus/HpBar/HP_Text");
 
      var text = textObj.GetComponent<TMP_Text>();
      text.text = "GAME OVER";
      text.fontSize = 30;
      text.color = Color.red;
 

  }
  //---------------------------------------------------------------------------
  private void GrayscaleScreen()
  {
    var volume = FindAnyObjectByType<UnityEngine.Rendering.Volume>();
    if (volume == null) return;

    // 런타임에서 ColorAdjustments가 없으면 추가
    if (!volume.profile.Has<UnityEngine.Rendering.Universal.ColorAdjustments>())
    {
      volume.profile.Add<UnityEngine.Rendering.Universal.ColorAdjustments>();
    }

    if (volume.profile.TryGet<UnityEngine.Rendering.Universal.ColorAdjustments>(out var colorAdj))
    {
      colorAdj.active = true;
      colorAdj.saturation.overrideState = true;
      colorAdj.saturation.value = -100f;
    }
  }
  //--------------------------------------------------------------------------- 
  public void SetMaxHP(float hp)
  {
    maxHP = hp;
    currentHP = maxHP;
    UpdateHPUI();
  }
  //---------------------------------------------------------------------------
#if UNITY_EDITOR
private void OnDisable()
{
    if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
    {
        if (SaveData.Instance != null)
            HardcoreReset();
    }
}
#endif
}