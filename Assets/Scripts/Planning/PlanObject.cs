using UnityEngine;

public abstract class PlanObject : MonoBehaviour
{
    public override string ToString()
    {
        Color color = Color.white;

        // Try SpriteRenderer first (common in games)
        if (TryGetComponent<SpriteRenderer>(out var spriteRenderer))
        {
            color = spriteRenderer.color;
        }
        // Fallback to Mesh Renderer
        else if (TryGetComponent<Renderer>(out var renderer))
        {
            if (renderer.sharedMaterial.HasProperty("_Color"))
                color = renderer.sharedMaterial.color;
        }

        string hex = ColorUtility.ToHtmlStringRGB(color);
        return $"<color=#{hex}><b>{gameObject.name}</b></color>";
    }


}