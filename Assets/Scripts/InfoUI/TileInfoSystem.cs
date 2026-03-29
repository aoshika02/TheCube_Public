using UnityEngine;
using R3;
using VContainer;

public class TileInfoSystem : MonoBehaviour
{
    private InputManager _inputManager;
    private GameStateManager _gameStateManager;
    private TileInfoModel _tileInfoModel;
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
            if (state == GameState.InGameShutdown)
            {
                _tileInfoView.ClearInfoViews();
            }
        }).AddTo(this);

        _tileInfoModel.OnAddInfo.Subscribe(infoList =>
        {
            _tileInfoView.AddInfoViews(infoList);
            _tileInfoView.SetActiveInfo(false);
        }).AddTo(this);

        _inputManager.KeyE.Subscribe(x =>
        {
            if (x != 1) return;
            if(_gameStateManager.State.CurrentValue != GameState.InGameIdle) return;
            if (_gameStateManager.InputState.CurrentValue != GameInputState.Other) return;
            _tileInfoView.SetActiveInfo();
        }).AddTo(this);
    }
}
