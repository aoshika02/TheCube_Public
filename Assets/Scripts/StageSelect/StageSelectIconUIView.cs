using UnityEngine;

public class StageSelectIconUIView : MonoBehaviour
{
    [SerializeField] private IconUI _decideUI;
    [SerializeField] private IconUI _jumpPanelIcon;
    [SerializeField] private Sprite _viewPanelIconTexture;
    [SerializeField] private Sprite _hidePanelIconTexture;

    public void ShowJumpPanel(bool isShow)
    {
        _jumpPanelIcon.SetTexture(isShow ? _hidePanelIconTexture: _viewPanelIconTexture);
        _decideUI.gameObject.SetActive(isShow);
    }

    public void ShowIcons(bool isShow)
    {
        gameObject.SetActive(isShow);
    }
}
