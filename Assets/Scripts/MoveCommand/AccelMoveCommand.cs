using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class AccelMoveCommand : MoveCommandBase
{
    public override async UniTask DoAsync(Transform target, MoveInfo moveInfo, CancellationToken token)
    {
        if (moveInfo?.TargetPosition.HasValue == false)
        {
            return;
        }
        await CubeMoveUtil.AccelMoveAsync(target, moveInfo.TargetPosition.Value, moveInfo.MoveDuration, token);
    }

    public override async UniTask UndoAsync(Transform target, MoveInfo moveInfo, CancellationToken token)
    {
        await CubeMoveUtil.AccelMoveAsync(target, moveInfo.OriginPosition, moveInfo.MoveDuration, token);
    }
}
