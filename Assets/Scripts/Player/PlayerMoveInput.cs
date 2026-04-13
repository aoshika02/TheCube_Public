using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;
using VContainer;

public class PlayerMoveInput : MonoBehaviour
{
    private GameStateManager _gameStateManager;
    private InputManager _inputManager;
    private PlayerMoveProcessor _playerMoveProcessor;

    [Inject]
    public void Construct(
        GameStateManager gameStateManager,
        InputManager inputManager,
        PlayerMoveProcessor playerMoveProcessor)
    {
        _gameStateManager = gameStateManager;
        _playerMoveProcessor = playerMoveProcessor;
        _inputManager = inputManager;
        Bind();
    }
    private void Bind()
    {
        _inputManager.KeyW
            .Where(x => x == 1)
            .SubscribeAwait(async (x, ct) =>
        {
            await OnInput(Vector2Int.up);
        }, AwaitOperation.Drop).AddTo(this);

        _inputManager.KeyS
            .Where(x => x == 1)
            .SubscribeAwait(async (x, ct) =>
        {
            await OnInput(Vector2Int.down);
        }, AwaitOperation.Drop).AddTo(this);

        _inputManager.KeyA
            .Where(x => x == 1)
            .SubscribeAwait(async (x, ct) =>
        {
            await OnInput(Vector2Int.left);
        }, AwaitOperation.Drop).AddTo(this);

        _inputManager.KeyD
            .Where(x => x == 1)
            .SubscribeAwait(async (x, ct) =>
        {
            await OnInput(Vector2Int.right);
        }, AwaitOperation.Drop).AddTo(this);
    }

    private async UniTask OnInput(Vector2Int moveDir)
    {
        if (_gameStateManager.State.CurrentValue != GameState.InGameIdle) return;
        if (_gameStateManager.InputState.CurrentValue != GameInputState.Other) return;
        _gameStateManager.ChangeInputState(GameInputState.Moving);
        var result = await _playerMoveProcessor.OnMove(moveDir, destroyCancellationToken);
        switch(result)
        {
            case ResultType.None:
                _gameStateManager.ChangeInputState(GameInputState.Other);
                break;
            case ResultType.Goal:
                _gameStateManager.ChangeState(GameState.InGameShutdown);
                break;
            case ResultType.Reset:
                _gameStateManager.ChangeState(GameState.InGameReset);
                break;
        }
    }
}
