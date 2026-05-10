using UnityEngine;
using TMPro;

public class WaveUI : MonoBehaviour
{
    private TextMeshProUGUI waveText;

    void Start()
    {
        waveText = GameObject.Find("PlayerHPCanvas/WaveContainer/Wave_Text")
                             .GetComponent<TextMeshProUGUI>();
        UpdateUI(0);
    }

    public void UpdateUI(int wave)
    {
        waveText.text = $"WAVE {wave + 1}";
    }
}