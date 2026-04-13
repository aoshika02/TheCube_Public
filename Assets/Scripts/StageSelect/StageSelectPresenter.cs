using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;
using VContainer;

public class StageSelectPresenter : MonoBehaviour
{
    private InputManager _inputManager;
    private StageSelectModel _model;
    private GameStateManager _gameStateManager;
    [SerializeField] private StageSelectView _view;

    [Inject]
    public void Construct(
        GameStateManager gameStateManager,
        InputManager inputManager,
        TilePool tilePool,
        TileManager tileManager,
        DataSetLoader dataLoader,
        PlayerCube playerCube,
        FadeManager fadeManager,
        StageSelectModel stagePreviewModel)
    {
        _gameStateManager = gameStateManager;
        _inputManager = inputManager;
        _model = stagePreviewModel;
        _view.Initialized(tilePool, tileManager, dataLoader, playerCube, fadeManager, gameStateManager, stagePreviewModel.StageID.CurrentValue);
        _view.SetCameraActive(false);
        Bind();
    }

    private void Bind()
    {
        _gameStateManager.State.Subscribe(async state =>
        {
            if (state == GameState.StageSelectInit)
            {
                var id = _model.StageID.CurrentValue;
                if (_model.GetStageSaveData(id, out var stageSaveData))
                {
                    _view.SetStagePreview(stageSaveData.StageID, stageSaveData.IsCleared);
                }
                else
                {
                    _view.SetStagePreview(id, false);
                }

                _view.SetCameraActive(true);
                await _view.SceneChangeFlow(id, true, destroyCancellationToken);
                _gameStateManager.ChangeState(GameState.StageSelectIdle);
                _gameStateManager.ChangeInputState(GameInputState.Other);
                _gameStateManager.ChangeSubState(SubGameState.Other);
            }
            else if (state == GameState.StageSelectShutdown)
            {
                _view.ShutDown();
                _gameStateManager.ChangeState(GameState.InGameInit);
            }
        }).AddTo(this);

        _model.OnIndexUpdate.Subscribe(async stageSaveData =>
        {
            if (_gameStateManager.State.CurrentValue != GameState.StageSelectIdle) return;
            if (_gameStateManager.SubState.CurrentValue == SubGameState.SelectJump)
            {
                await _view.SelectJumpFlow(stageSaveData, destroyCancellationToken);
            }
            else
            {
                await _view.UpdateStagePreviewAsync(stageSaveData, destroyCancellationToken);
            }
            _gameStateManager.ChangeInputState(GameInputState.Other);

        }).AddTo(this);

        _inputManager.KeyW.Where(x => x == 1).Subscribe(async _ =>
        {
            if (IsInputEnabled() == false) return;
            _gameStateManager.ChangeInputState(GameInputState.Moving);
            await _view.SceneChangeFlow(_model.StageID.CurrentValue, false, destroyCancellationToken);
            _gameStateManager.ChangeState(GameState.StageSelectShutdown);
        }).AddTo(this);

        _inputManager.KeyS.Where(x => x == 1).Subscribe(async _ =>
        {
            if (IsInputEnabled() == false) return;
            _gameStateManager.ChangeInputState(GameInputState.Moving);
            await _view.UnMoveFlow(destroyCancellationToken);
            _gameStateManager.ChangeInputState(GameInputState.Other);
        }).AddTo(this);

        _inputManager.KeyA.Where(x => x == 1).Subscribe(_ =>
        {
            if (IsInputEnabled() == false) return;
            if (_model.StageID.CurrentValue <= _model.MinStageID) return;
            _gameStateManager.ChangeInputState(GameInputState.Moving);
            _model.SubtractStageID();
            _gameStateManager.RequestInputUIRefresh();
        }).AddTo(this);

        _inputManager.KeyD.Where(x => x == 1).Subscribe(_ =>
        {
            if (IsInputEnabled() == false) return;
            if (_model.StageID.CurrentValue >= _model.MaxStageID) return;
            _gameStateManager.ChangeInputState(GameInputState.Moving);
            _model.AddStageID();
            _gameStateManager.RequestInputUIRefresh();
        }).AddTo(this);

        _model.OnClear.Subscribe(_ =>
        {
            _view.OnClear(_);
        }).AddTo(this);
    }

    private bool IsInputEnabled()
    {
        if (_gameStateManager.State.CurrentValue != GameState.StageSelectIdle) return false;
        if (_gameStateManager.InputState.CurrentValue != GameInputState.Other) return false;
        if (_gameStateManager.SubState.CurrentValue != SubGameState.Other) return false;
        return true;
    }
}
