using UnityEngine;
using VContainer;
using R3;

public class SelectJumpPresenter : MonoBehaviour
{
    private GameStateManager _gameStateManager;
    private InputManager _inputManager;
    private SelectJumpModel _model;
    [SerializeField] private SelectJumpView _view;

    [Inject]
    public void Construct(GameStateManager gameStateManager, InputManager inputManager, SelectJumpModel model)
    {
        _gameStateManager = gameStateManager;
        _inputManager = inputManager;
        _model = model;
        _view.gameObject.SetActive(false);
        Bind();
    }

    private void Bind()
    {
        _model.OnIndexUpdate.Subscribe(info =>
        {
            _view.UpdateSelectJumpColor(info);
            _gameStateManager.RequestInputUIRefresh();
        }).AddTo(this);

        _gameStateManager.SubState.Where(subState => subState != SubGameState.SelectJump).Subscribe(_ =>
        {
            _view.gameObject.SetActive(false);
        }).AddTo(this);

        _inputManager.KeyE.Where(x => x == 1).Subscribe(_ =>
        {
            if (IsInputEnabled() == false) return;
            _gameStateManager.ChangeInputState(GameInputState.None);
            _model.OnJump();
        }).AddTo(this);

        _inputManager.KeyQ.Where(x => x == 1).Subscribe(_ =>
        {
            if(_gameStateManager.State.CurrentValue != GameState.StageSelectIdle) return;
            if (_gameStateManager.InputState.CurrentValue != GameInputState.Other) return;
            if (_gameStateManager.SubState.CurrentValue == SubGameState.Other)
            {
                _gameStateManager.ChangeSubState(SubGameState.SelectJump);
                _view.gameObject.SetActive(true);
                return;
            }
            if (_gameStateManager.SubState.CurrentValue == SubGameState.SelectJump)
            {
                _view.gameObject.SetActive(false);
                _gameStateManager.ChangeSubState(SubGameState.Other);
            }
        }).AddTo(this);

        _inputManager.KeyW.Where(x => x == 1).Subscribe(_ => OnCursorMove(Vector2Int.up)).AddTo(this);

        _inputManager.KeyS.Where(x => x == 1).Subscribe(_ => OnCursorMove(Vector2Int.down)).AddTo(this);

        _inputManager.KeyA.Where(x => x == 1).Subscribe(_ => OnCursorMove(Vector2Int.left)).AddTo(this);

        _inputManager.KeyD.Where(x => x == 1).Subscribe(_ => OnCursorMove(Vector2Int.right)).AddTo(this);
    }

    private void OnCursorMove(Vector2Int input)
    {
        if (IsInputEnabled() == false) return;
        if (input == Vector2Int.zero) return;
        _model.ChangeSelectID(input);
    }

    private bool IsInputEnabled()
    {
        if (_gameStateManager.State.CurrentValue != GameState.StageSelectIdle) return false;
        if (_gameStateManager.SubState.CurrentValue != SubGameState.SelectJump) return false;
        if (_gameStateManager.InputState.CurrentValue != GameInputState.Other) return false;
        return true;
    }
}
