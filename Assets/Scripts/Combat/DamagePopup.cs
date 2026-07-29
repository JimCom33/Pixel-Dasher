using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private const float Lifetime = 0.8f;
    private const float RiseSpeed = 1.5f;

    private TextMesh mainText;
    private TextMesh shadowText;
    private float elapsedTime;

    public static void Spawn(Vector3 position, int amount)
    {
        GameObject popupObject = new GameObject($"Damage Popup ({amount})");
        popupObject.transform.position = position;

        DamagePopup popup = popupObject.AddComponent<DamagePopup>();
        popup.CreateText(amount.ToString());
    }

    private void CreateText(string value)
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        shadowText = CreateTextLayer(
            "Shadow",
            value,
            font,
            Color.black,
            new Vector3(0.04f, -0.04f, 0f),
            199);
        mainText = CreateTextLayer(
            "Damage",
            value,
            font,
            new Color(1f, 0.85f, 0.1f),
            Vector3.zero,
            200);
    }

    private TextMesh CreateTextLayer(
        string objectName,
        string value,
        Font font,
        Color color,
        Vector3 localPosition,
        int sortingOrder)
    {
        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(transform, false);
        textObject.transform.localPosition = localPosition;

        TextMesh textMesh = textObject.AddComponent<TextMesh>();
        textMesh.text = value;
        textMesh.font = font;
        textMesh.fontSize = 64;
        textMesh.characterSize = 0.08f;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.color = color;

        MeshRenderer meshRenderer = textObject.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = font.material;
        meshRenderer.sortingOrder = sortingOrder;
        return textMesh;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        transform.position += Vector3.up * (RiseSpeed * Time.deltaTime);

        float alpha = 1f - Mathf.Clamp01(elapsedTime / Lifetime);
        SetAlpha(mainText, alpha);
        SetAlpha(shadowText, alpha);

        if (elapsedTime >= Lifetime)
        {
            Destroy(gameObject);
        }
    }

    private static void SetAlpha(TextMesh textMesh, float alpha)
    {
        Color color = textMesh.color;
        color.a = alpha;
        textMesh.color = color;
    }
}
