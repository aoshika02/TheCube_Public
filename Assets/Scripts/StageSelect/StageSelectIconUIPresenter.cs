using UnityEngine;
using VContainer;
using R3;

public class StageSelectIconUIPresenter : MonoBehaviour
{
    [SerializeField] private StageSelectIconUIView _view;
    private GameStateManager _gameStateManager;

    [Inject]
    public void Construct(GameStateManager gameStateManager)
    {
        _gameStateManager = gameStateManager;
        _view.ShowIcons(false);
        _view.ShowJumpPanel(false);
        Bind();
    }

    private void Bind()
    {
        _gameStateManager.State
            .Subscribe(state =>
            {
                if (state == GameState.StageSelectInit)
                {
                    _view.ShowIcons(true);
                    return;
                }

                if (state == GameState.StageSelectShutdown)
                {
                    _view.ShowIcons(false);
                }
            }).AddTo(this);

        _gameStateManager.SubState
            .Subscribe(subState =>
            {
                if (subState == SubGameState.SelectJump)
                {
                    _view.ShowJumpPanel(true);
                    return;
                }
                else if (subState == SubGameState.Other)
                {
                    _view.ShowJumpPanel(false);
                }
            }).AddTo(this);
    }
}
