using UnityEngine;

public class DebugDisplay : MonoBehaviour
{
  [SerializeField] private bool showInGame = true;
  [SerializeField] private int fontSize = 22;
  private float deltaTime = 0f;
  private int displayFPS = 0;
  private float timer = 0f;

  void Update()
  {
    deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    timer += Time.unscaledDeltaTime;
    if (timer >= 1f)
    {
      displayFPS = Mathf.CeilToInt(1.0f / deltaTime);
      timer = 0f;
    }
    
  }

  void OnGUI()
  {    
    //int batches = UnityEngine.Rendering.DebugManager.instance != null ? 0 : 0;

    // 배경 박스
    GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
    boxStyle.fontSize = fontSize;
    boxStyle.normal.textColor = Color.white;
    boxStyle.alignment = TextAnchor.UpperLeft;
    boxStyle.padding = new RectOffset(8, 8, 4, 4);

    // FPS 색상 (60+ 초록, 30+ 노랑, 30미만 빨강)
    string fpsColor = displayFPS >= 60 ? "lime" : displayFPS >= 30 ? "yellow" : "red";

    string text = $"<color={fpsColor}>FPS: {displayFPS}</color>\n";
    text += $"Frame: {Time.frameCount}\n";
    text += $"DeltaTime: {deltaTime * 1000f:F1}ms";

    GUI.Box(new Rect(Screen.width - 220, 10, 210, 100), text, boxStyle);
  }
}