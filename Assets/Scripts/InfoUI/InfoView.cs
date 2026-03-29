using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoView : MonoBehaviour, IPool
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _infoText;

    public bool IsGenericUse { get; set; }
    public void SetInfo(Texture2D iconTex, Color color, string info)
    {
        //テクスチャの設定は固定であるためマジックナンバーを使用
        _iconImage.sprite = Sprite.Create(
                            iconTex,
                            new Rect(0, 0, iconTex.width, iconTex.height),
                            new Vector2(0.5f, 0.5f)
        ); 
        _iconImage.color = color;
        _infoText.text = info;
    }

    public void OnRelease()
    {
        gameObject.SetActive(false);
    }

    public void OnReuse()
    {
        gameObject.SetActive(true);
    }
}