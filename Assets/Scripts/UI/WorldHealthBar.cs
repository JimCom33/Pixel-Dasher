using UnityEngine;

[RequireComponent(typeof(Health))]
public class WorldHealthBar : MonoBehaviour
{
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1.25f, 0f);
    [SerializeField] private Vector2 size = new Vector2(90f, 14f);
    [SerializeField] private Color backgroundColor = new Color(0.08f, 0.08f, 0.08f, 0.9f);
    [SerializeField] private Color fillColor = new Color(0.8f, 0.08f, 0.08f, 1f);

    private Health health;
    private Camera mainCamera;
    private float currentHealth;
    private float maxHealth = 1f;
    private GUIStyle labelStyle;

    private void Awake()
    {
        health = GetComponent<Health>();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        if (health == null)
        {
            health = GetComponent<Health>();
        }

        health.onHealthChanged.AddListener(UpdateHealth);
    }

    private void Start()
    {
        UpdateHealth(health.GetCurrentHealth(), health.GetMaxHealth());
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
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null)
        {
            return;
        }

        Vector3 screenPosition = mainCamera.WorldToScreenPoint(transform.position + worldOffset);
        if (screenPosition.z <= 0f)
        {
            return;
        }

        float left = screenPosition.x - size.x * 0.5f;
        float top = Screen.height - screenPosition.y - size.y * 0.5f;
        Rect backgroundRect = new Rect(left, top, size.x, size.y);
        const float border = 2f;
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
            fontSize = 10,
            normal = { textColor = Color.white }
        };

        string healthText = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
        GUI.Label(backgroundRect, healthText, labelStyle);
    }
}
