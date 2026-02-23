using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class AlphaExtensions
{
    #region UI

    #region ColorParameter
    public static void SetAlpha(this Image self, float alpha)
    {
        Color color = self.color;
        color.a = alpha;
        self.color = color;
    }
    public static void SetAlpha(this RawImage self, float alpha)
    {
        Color color = self.color;
        color.a = alpha;
        self.color = color;
    }
    public static void SetAlpha(this TextMeshProUGUI self, float alpha)
    {
        Color color = self.color;
        color.a = alpha;
        self.color = color;
    }
    #endregion

    #region MaterialParameter
    public static void SetMaterialAlpha(this Image image, float alpha)
    {
        if (image.material != null)
        {
            var c = image.material.color;
            c.a = alpha;
            image.material.color = c;
        }
    }
    public static void SetMaterialAlpha(this RawImage rawImage, float alpha)
    {
        if (rawImage.material != null)
        {
            var c = rawImage.material.color;
            c.a = alpha;
            rawImage.material.color = c;
        }
    }
    public static void SetMaterialAlpha(this TextMeshProUGUI textMeshPro, float alpha)
    {
        if (textMeshPro.material != null)
        {
            var c = textMeshPro.material.color;
            c.a = alpha;
            textMeshPro.material.color = c;
        }
    }
    #endregion

    #endregion

    #region 2D
    public static void SetAlpha(this SpriteRenderer self, float alpha)
    {
        Color color = self.color;
        color.a = alpha;
        self.color = color;
    }
    public static void SetMaterialAlpha(this SpriteRenderer spriteRenderer, float alpha)
    {
        if (spriteRenderer.material != null)
        {
            var c = spriteRenderer.material.color;
            c.a = alpha;
            spriteRenderer.material.color = c;
        }
    }
    #endregion

    #region 3D

    public static void SetAlpha(this MeshRenderer self, float alpha)
    {
        if (self.materials != null && self.materials.Length != 0)
        {
            self.materials.SetMaterialAlpha(alpha);
        }
    }
    public static void SetAlpha(this SkinnedMeshRenderer self, float alpha)
    {
        if (self.materials != null && self.materials.Length != 0)
        {
            self.materials.SetMaterialAlpha(alpha);
        }
    }
    #endregion

    #region Material
    public static void SetAlpha(this Material self, float alpha)
    {
        Color color = self.color;
        color.a = alpha;
        self.color = color;
    }
    public static void SetMaterialAlpha(this Material[] materials, float alpha)
    {
        foreach (var mat in materials)
        {
            var c = mat.color;
            c.a = alpha;
            mat.color = c;
        }
    }
    #endregion
}
