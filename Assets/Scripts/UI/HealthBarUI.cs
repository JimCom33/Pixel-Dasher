using UnityEngine;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Vector2 position = new Vector2(24f, 24f);
    [SerializeField] private Vector2 size = new Vector2(260f, 30f);
    [SerializeField] private Color backgroundColor = new Color(0.08f, 0.08f, 0.08f, 0.9f);
    [SerializeField] private Color fillColor = new Color(0.8f, 0.08f, 0.08f, 1f);

    private float currentHealth;
    private float maxHealth = 1f;
    private GUIStyle labelStyle;

    private void OnEnable()
    {
        if (health != null)
        {
            health.onHealthChanged.AddListener(UpdateHealth);
        }
    }

    private void Start()
    {
        if (health != null)
        {
            UpdateHealth(health.GetCurrentHealth(), health.GetMaxHealth());
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.onHealthChanged.RemoveListener(UpdateHealth);
        }
    }

    private void UpdateHealth(float newCurrentHealth, float newMaxHealth)
    {
        currentHealth = newCurrentHealth;
        maxHealth = Mathf.Max(1f, newMaxHealth);
    }

    private void OnGUI()
    {
        const float border = 4f;
        Rect backgroundRect = new Rect(position.x, position.y, size.x, size.y);
        float healthPercent = Mathf.Clamp01(currentHealth / maxHealth);
        Rect fillRect = new Rect(
            backgroundRect.x + border,
            backgroundRect.y + border,
            (backgroundRect.width - border * 2f) * healthPercent,
            backgroundRect.height - border * 2f);

        Color previousColor = GUI.color;
        GUI.color = backgroundColor;
        GUI.DrawTexture(backgroundRect, Texture2D.whiteTexture);
        GUI.color = fillColor;
        GUI.DrawTexture(fillRect, Texture2D.whiteTexture);
        GUI.color = previousColor;

        labelStyle ??= new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = 16,
            normal = { textColor = Color.white }
        };

        string healthText = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
        GUI.Label(backgroundRect, healthText, labelStyle);
    }
}
