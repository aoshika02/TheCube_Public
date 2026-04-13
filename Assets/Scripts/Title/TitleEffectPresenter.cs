using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using R3;
using VContainer;

public class TitleEffectPresenter : MonoBehaviour
{
    private GameStateManager _gameStateManager;
    [SerializeField] private TitleEffectView _titleEffectView;

    [Inject]
    public void Construct(GameStateManager gameStateManager, PlayerCube playerCube)
    {
        _gameStateManager = gameStateManager;
        _titleEffectView.Init(playerCube);
        Bind();
    }

    private void Bind()
    {
        var cts = new CancellationTokenSource();
        _gameStateManager.State.Subscribe(state =>
        {
            if (state == GameState.TitleInit)
            {
                _titleEffectView.CameraRotInit();
                _titleEffectView.SetEffectRootActive(true);
                _titleEffectView.SetCameraActive(true);


                _titleEffectView.TitleEffectFlow(cts.Token).Forget();
            }

            if(state == GameState.TitleShutdown)
            {
                cts.Cancel();
                _titleEffectView.SetEffectRootActive(false);
                _titleEffectView.SetCameraActive(false);
                _gameStateManager.ChangeState(GameState.StageSelectInit);
            }

        }).AddTo(this);
    }
}