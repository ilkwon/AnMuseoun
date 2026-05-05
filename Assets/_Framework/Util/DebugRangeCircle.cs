using UnityEngine;

public class DebugRangeCircle : MonoBehaviour
{
    [SerializeField] private float radius = GameConst.AttackRange;
    [SerializeField] private Color color = Color.red;
    [SerializeField] private bool showInGame = true;

    private LineRenderer line;

    void Start()
    {
        if (!showInGame) return;

        line = gameObject.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.loop = true;
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.positionCount = 36;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = color;
        line.endColor = color;

        DrawCircle();
    }

    void DrawCircle()
    {
        for (int i = 0; i < 36; i++)
        {
            float angle = i * 10f * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            line.SetPosition(i, new Vector3(x, 0.1f, z));
        }
    }
}