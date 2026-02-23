using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class IconUIManager : SingletonMonoBehaviour<IconUIManager>
{
    [SerializeField] private List<IconUI> _iconUIs;
    [SerializeField] private Color _activeIconColor = Color.white;
    [SerializeField] private Color _deactiveColor = Color.gray;

    private GameInputStateManager _gameInputStateManager;
    
    public void Init()
    {
        _gameInputStateManager = GameInputStateManager.Instance;
        _gameInputStateManager.OnInputStateChanged
            .StartWith(_gameInputStateManager.CurrentInputState)
            .Subscribe(x =>
            {
                if (x == GameInputState.Other)
                {
                    SetIconColor(new IconColorData(_activeIconColor, _activeIconColor));
                }
                else
                {
                    SetIconColor(new IconColorData(_deactiveColor, _deactiveColor));
                }
            }).AddTo(this);
    }

    private void SetIconColor(IconColorData iconColorData)
    {
        foreach (var iconUI in _iconUIs)
        {
            iconUI.SetColor(iconColorData);
        }
    }
}
