using UnityEngine;
using UnityEngine.UI;

public class IconUI : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private Image _keyBG;

    public void SetColor(IconColorData iconColorData) 
    {
        _iconImage.color = iconColorData.IconColor;
        _keyBG.color = iconColorData.KeyBGColor;
    }
}

public record IconColorData 
{
    public readonly Color IconColor;
    public readonly Color KeyBGColor;

    public IconColorData(Color iconColor, Color keyBGColor)
    {
        IconColor = iconColor;
        KeyBGColor = keyBGColor;
    }
}