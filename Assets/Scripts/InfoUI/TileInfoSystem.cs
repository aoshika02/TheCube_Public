using UnityEngine;
using R3;
using VContainer;
using System.Threading;

public class TileInfoSystem : MonoBehaviour
{
    private InputManager _inputManager;
    private GameStateManager _gameStateManager;
    private TileInfoModel _tileInfoModel;
    private CancellationTokenSource _cts = new CancellationTokenSource();
    [SerializeField] private TileInfoView _tileInfoView;

    [Inject]
    public void Construct(
        TileInfoModel tileInfoModel,
        InputManager inputManager,
        GameStateManager gameStateManager,
        DataSetLoader dataSetLoader,
        TextDataset textDataset,
        InfoViewPool infoViewPool)
    {
        _inputManager = inputManager;
        _tileInfoModel = tileInfoModel;
        _gameStateManager = gameStateManager;
        _tileInfoView.Initialized(infoViewPool, dataSetLoader, textDataset);
        Bind();
    }

    private void Bind()
    {
        _gameStateManager.State.Subscribe(state =>
        {
            if (state == GameState.InGameInit)
            {
                _cts?.Cancel();
                _cts?.Dispose();
                _cts = new CancellationTokenSource();
            }

            if (state == GameState.InGameShutdown)
            {
                _tileInfoView.ClearInfoViews();
                _cts?.Cancel();
                _cts?.Dispose();
                _cts = null;
            }
        }).AddTo(this);

        _tileInfoModel.OnAddInfo.Subscribe(async info =>
        {
            await _tileInfoView.AddInfoViews(info.TileInfoKeys, _cts.Token);
            await _tileInfoView.SetActiveInfo(!info.IsVerified, _cts.Token);
        }).AddTo(this);

        _inputManager.KeyE.Where(x => x == 1)
        .SubscribeAwait(async (x, ct) =>
        {
            if (_gameStateManager.State.CurrentValue != GameState.InGameIdle) return;
            if (_gameStateManager.InputState.CurrentValue != GameInputState.Other) return;
            _gameStateManager.ChangeInputState(GameInputState.Moving);
            await _tileInfoView.SetActiveInfo(token: _cts.Token);
            _gameStateManager.ChangeInputState(GameInputState.Other);
        }, AwaitOperation.Drop)
        .RegisterTo(destroyCancellationToken);
    }
}
