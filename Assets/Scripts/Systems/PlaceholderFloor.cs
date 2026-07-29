using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class PlaceholderFloor : MonoBehaviour
{
    private Sprite generatedSprite;
    private Texture2D generatedTexture;

    private void OnEnable()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer.sprite != null)
        {
            return;
        }

        generatedTexture = new Texture2D(1, 1)
        {
            name = "Generated Black Floor Texture",
            filterMode = FilterMode.Point,
            hideFlags = HideFlags.HideAndDontSave
        };
        generatedTexture.SetPixel(0, 0, Color.black);
        generatedTexture.Apply();

        generatedSprite = Sprite.Create(
            generatedTexture,
            new Rect(0f, 0f, 1f, 1f),
            new Vector2(0.5f, 0.5f),
            1f);
        generatedSprite.name = "Generated Black Floor Sprite";
        generatedSprite.hideFlags = HideFlags.HideAndDontSave;
        spriteRenderer.sprite = generatedSprite;
        spriteRenderer.color = Color.white;
    }

    private void OnDisable()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite == generatedSprite)
        {
            spriteRenderer.sprite = null;
        }

        DestroyGeneratedObject(generatedSprite);
        DestroyGeneratedObject(generatedTexture);
        generatedSprite = null;
        generatedTexture = null;
    }

    private static void DestroyGeneratedObject(Object generatedObject)
    {
        if (generatedObject == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            Destroy(generatedObject);
        }
        else
        {
            DestroyImmediate(generatedObject);
        }
    }
}
