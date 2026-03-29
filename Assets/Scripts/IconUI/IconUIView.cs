using System.Collections.Generic;
using UnityEngine;

public class IconUIView : MonoBehaviour
{
    [SerializeField] private List<IconUI> _iconUIs;
    [SerializeField] private Color _activateIconColor = Color.white;
    [SerializeField] private Color _deactivateColor = Color.gray;
    private IconColorData _activateIconColorData;
    private IconColorData _deactivateIconColorData;

    public void Initialized()
    {
        _activateIconColorData = new IconColorData(_activateIconColor, _activateIconColor);
        _deactivateIconColorData = new IconColorData(_deactivateColor, _deactivateColor);
    }

    public void SetIconColor(bool isActivate)
    {
        if (isActivate)
        {
            SetIconColor(_activateIconColorData);
        }
        else
        {
            SetIconColor(_deactivateIconColorData);
        }
    }

    private void SetIconColor(IconColorData iconColorData)
    {
        foreach (var iconUI in _iconUIs)
        {
            iconUI.SetColor(iconColorData);
        }
    }

    public void SetActiveIcons(bool isActive)
    {
        foreach (var iconUI in _iconUIs)
        {
            if (iconUI.gameObject.activeSelf != isActive)
                iconUI.gameObject.SetActive(isActive);
        }
    }
}
