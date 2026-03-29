using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;
using VContainer;

public class TitlePresenter : MonoBehaviour
{
    private GameStateManager _gameStateManager;
    private FadeManager _fadeManager;
    private InputManager _inputManager;
    private SaveManager _saveManager;
    private LoadManager _loadManager;
    private DialogModel _dialogModel;
    private bool _isLoading = false;
    [SerializeField] private GameObject _titleRoot;
    [SerializeField] private TitleCamera _titleCamera;

    [Inject]
    public void Construct(
        GameStateManager gameStateManager,
        FadeManager fadeManager,
        SaveManager saveManager,
        LoadManager loadManager,
        InputManager inputManager,
        DialogModel dialogModel)
    {
        _gameStateManager = gameStateManager;
        _fadeManager = fadeManager;
        _saveManager = saveManager;
        _loadManager = loadManager;
        _inputManager = inputManager;
        _dialogModel = dialogModel;
        Bind();
    }

    private void Bind()
    {
        _gameStateManager.State.Subscribe(async state =>
        {
            if (state == GameState.TitleInit)
            {
                _titleCamera.SetCameraActive(true);
                _titleRoot.SetActive(true);
                await _fadeManager.FadeIn(token: destroyCancellationToken);
                _gameStateManager.ChangeState(GameState.TitleIdle);
                _gameStateManager.SetInputState(GameInputState.Other);
            }
            else if (state == GameState.TitleShutdown)
            {
                _titleRoot.SetActive(false);
                _titleCamera.SetCameraActive(false);
            }
            if (state != GameState.TitleIdle) return;
            _isLoading = false;
        }).AddTo(this);

        _inputManager.Space.Subscribe(async x =>
        {
            if (x != 1) return;
            if (_gameStateManager.InputState.CurrentValue != GameInputState.Other) return;
            if (_gameStateManager.State.CurrentValue != GameState.TitleIdle) return;
            if (_isLoading) return;
            _isLoading = true;
            await _fadeManager.FadeOut();
            _gameStateManager.ChangeState(GameState.TitleShutdown);
        }).AddTo(this);

        _inputManager.KeyX.Subscribe(x =>
        {
            if (x != 1) return;
            if (_gameStateManager.InputState.CurrentValue != GameInputState.Other) return;
            if (_saveManager.DeleteSaveData())
            {
                _loadManager.ClearCache();
                _dialogModel.AddDialog(DialogEventType.DeleteSaveData);
            }
        }).AddTo(this);
    }
}
