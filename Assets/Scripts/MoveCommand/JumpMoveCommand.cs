using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class JumpMoveCommand : MoveCommandBase
{
    private readonly PlayerCube _playerCube;
    private float _spawnPosY = 11;
    public override async UniTask DoAsync(Transform target, MoveInfo moveInfo, CancellationToken token)
    {
        if (moveInfo?.TargetPosition.HasValue == false)
        {
            return;
        }
        await CubeMoveUtil.JumpMoveAsync(target, moveInfo.TargetPosition.Value, moveTime: moveInfo.MoveDuration, token: token);

        if (moveInfo.TargetTileType.HasValue == false)
        {
            await CubeMoveUtil.MoveYAsync(target, _spawnPosY, moveInfo.MoveDuration, token);
        }
    }

    public override async UniTask UndoAsync(Transform target, MoveInfo moveInfo, CancellationToken token)
    {
        await CubeMoveUtil.JumpMoveAsync(target, moveInfo.OriginPosition, moveTime: moveInfo.MoveDuration, token: token);
    }
}
