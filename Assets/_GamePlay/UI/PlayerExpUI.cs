using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerExpUI : MonoBehaviour
{
  private Image expFill;
  private TextMeshProUGUI expText;
  private PlayerStateMachine playerSM;

  //---------------------------------------------------------------------------
  void Start()
  {
    expFill = GameObject.Find("UIRoot/PanelPlayStatus/ExpBar/EXP_Fill")
                        .GetComponent<Image>();
    expText = GameObject.Find("UIRoot/PanelPlayStatus/ExpBar/EXP_Text")
                        .GetComponent<TextMeshProUGUI>();
    playerSM = GetComponent<PlayerStateMachine>();

    UpdateUI();
  }

  //---------------------------------------------------------------------------
  public void UpdateUI()
  {
    var stat = GameDataManager.Instance.GetPlayerStat(playerSM.CurrentLevel);
    if (stat == null) return;

    float ratio = playerSM.CurrentEXP / stat.exp_required;
    expFill.rectTransform.anchorMax = new Vector2(ratio, 1f);
    expText.text = $"Lv.{playerSM.CurrentLevel}  {Mathf.FloorToInt(playerSM.CurrentEXP)}/{stat.exp_required}";
  }
  //---------------------------------------------------------------------------
}