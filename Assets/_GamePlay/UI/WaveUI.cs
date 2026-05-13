using UnityEngine;
using TMPro;
using System;

public class WaveUI : MonoBehaviour
{
  private TextMeshProUGUI waveText;
  //---------------------------------------------------------------------------
  void Start()
  {
    BindUI();

    EventBus.On<OnWaveChanged>(OnWaveChanged);
  }
  //---------------------------------------------------------------------------
  private void OnWaveChanged(OnWaveChanged e)
  {
    UpdateWaveText(e.wave);
  }

  //---------------------------------------------------------------------------
  private void BindUI()
  {
    waveText = GameObject.Find("UIRoot/PanelPlayStatus/WaveStatus/Wave_Text").GetComponent<TextMeshProUGUI>();
  }

  //---------------------------------------------------------------------------
  public void UpdateWaveText(int wave)
  {
    waveText.text = $"WAVE {wave + 1}";
  }
  //---------------------------------------------------------------------------
  void OnDestroy()
  {
    EventBus.Off<OnWaveChanged>(OnWaveChanged);
  }

  //---------------------------------------------------------------------------
}