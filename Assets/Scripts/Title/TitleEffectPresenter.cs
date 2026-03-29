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
        _gameStateManager.State.Subscribe(state =>
        {
            if (state == GameState.TitleInit)
            {
                _titleEffectView.CameraRotInit();
                _titleEffectView.SetEffectRootActive(true);
                _titleEffectView.SetCameraActive(true);

                var cts = new CancellationTokenSource();

                _titleEffectView.TitleEffectFlow(cts.Token).Forget();

                _gameStateManager.State
                    .Skip(1)
                    .Where(s => s != GameState.TitleIdle)
                    .Take(1)
                    .Subscribe(_ => cts.Cancel())
                    .AddTo(this);
            }

            if(state == GameState.TitleShutdown)
            {
                _titleEffectView.SetEffectRootActive(false);
                _titleEffectView.SetCameraActive(false);
                _gameStateManager.ChangeState(GameState.StageSelectInit);
            }

        }).AddTo(this);
    }
}