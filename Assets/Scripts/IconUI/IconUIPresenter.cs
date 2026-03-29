using UnityEngine;
using R3;
using VContainer;

public class IconUIPresenter : MonoBehaviour
{
    private GameStateManager _gameStateManager;
    [SerializeField] private IconUIView _iconUIView;

    [Inject]
    public void Construct(GameStateManager gameInputStateManager)
    {
        _gameStateManager = gameInputStateManager;
        _iconUIView.Initialized();
        _iconUIView.SetActiveIcons(false);
        Bind();
    }

    private void Bind()
    {
        _gameStateManager.State.Subscribe(state =>
        {
            if(state == GameState.InGameInit)
            {
                _iconUIView.SetActiveIcons(true);
                return;
            }

            if (state == GameState.InGameShutdown)
            {
                _iconUIView.SetActiveIcons(false);
            }
        }).AddTo(this);

        _gameStateManager.InputState.Subscribe(inputState =>
        {
            if (_gameStateManager.State.CurrentValue == GameState.InGameIdle)
            {
                _iconUIView.SetIconColor(inputState == GameInputState.Other);
            }
        }).AddTo(this);
    }
}
