using UnityEngine;
using R3;
using VContainer;

public class InGamePresenter : MonoBehaviour
{
    private InputManager _inputManager;
    private GameStateManager _gameStateManager;
    private TutorialManager _tutorialManager;
    private InGameModel _model;
    private PlayerMoveProcessor _playerMoveProcessor;
    [SerializeField] private InGameView _view;

    [Inject]
    public void Construct(
        InputManager inputManager,
        GameStateManager gameStateManager,
        TutorialManager tutorialManager,
        InGameModel model,
        PlayerMoveProcessor playerMoveProcessor)
    {
        _inputManager = inputManager;
        _gameStateManager = gameStateManager;
        _tutorialManager = tutorialManager;
        _model = model;
        _playerMoveProcessor = playerMoveProcessor;
        Bind();
    }

    private void Bind()
    {
        _gameStateManager.State.Subscribe(async state =>
        {
            if (state == GameState.InGameInit)
            {
                _playerMoveProcessor.Initialize();
                _model.Initialize();
                await _view.InitializeAsync(_model.StartID, destroyCancellationToken);
                await _tutorialManager.TutorialFlow();
                _gameStateManager.ChangeState(GameState.InGameIdle);
                _gameStateManager.ChangeInputState(GameInputState.Other);
            }

            if (state == GameState.InGameShutdown)
            {
                _playerMoveProcessor.Reset();
                _model.ShutDown(_playerMoveProcessor.IsCleared());
                await _view.ShutDown(destroyCancellationToken);
                _gameStateManager.ChangeState(GameState.StageSelectInit);
            }

            if (state == GameState.InGameReset)
            {
                _playerMoveProcessor.Reset();
                _model.Reset();
                await _view.ResetAsync(destroyCancellationToken);
                _gameStateManager.ChangeState(GameState.InGameIdle);
            }
        }).AddTo(this);

        _inputManager.KeyQ.Subscribe(x =>
        {
            if (x != 1) return;
            if (_gameStateManager.State.CurrentValue != GameState.InGameIdle) return;
            if (_gameStateManager.InputState.CurrentValue != GameInputState.Other) return;
            _gameStateManager.ChangeState(GameState.InGameShutdown);
        }).AddTo(this);

        _inputManager.KeyR.Subscribe(x =>
        {
            if (x != 1) return;
            if (_gameStateManager.State.CurrentValue != GameState.InGameIdle) return;
            if (_gameStateManager.InputState.CurrentValue != GameInputState.Other) return;
            _gameStateManager.ChangeState(GameState.InGameReset);
        }).AddTo(this);
    }
}
