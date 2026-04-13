using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;
using VContainer;

public class TitlePresenter : MonoBehaviour
{
    private GameStateManager _gameStateManager;
    private FadeManager _fadeManager;
    private InputManager _inputManager;
    private TitleModel _model;
    private bool _isLoading = false;
    [SerializeField] private GameObject _titleRoot;
    [SerializeField] private TitleCamera _titleCamera;

    [Inject]
    public void Construct(
        GameStateManager gameStateManager,
        FadeManager fadeManager,
        InputManager inputManager,
        TitleModel model)
    {
        _gameStateManager = gameStateManager;
        _fadeManager = fadeManager;
        _inputManager = inputManager;
        _model = model;
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
                _gameStateManager.ChangeInputState(GameInputState.Other);
            }
            else if (state == GameState.TitleShutdown)
            {
                _titleRoot.SetActive(false);
                _titleCamera.SetCameraActive(false);
            }
            if (state != GameState.TitleIdle) return;
            _isLoading = false;
        }).AddTo(this);

        _inputManager.Space.Where(x => x == 1).SubscribeAwait(async (x, ct) =>
        {
            if (_gameStateManager.InputState.CurrentValue != GameInputState.Other) return;
            if (_gameStateManager.State.CurrentValue != GameState.TitleIdle) return;
            if (_isLoading) return;
            _isLoading = true;
            await _fadeManager.FadeOut();
            _gameStateManager.ChangeState(GameState.TitleShutdown);
        }, AwaitOperation.Drop).AddTo(this);

        _inputManager.KeyX.Where(x => x == 1).Subscribe(x =>
        {
            if (_gameStateManager.InputState.CurrentValue != GameInputState.Other) return;
            _model.DeleteSaveData();
        }).AddTo(this);
    }
}
