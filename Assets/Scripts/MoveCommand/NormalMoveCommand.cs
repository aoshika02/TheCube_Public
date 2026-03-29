using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class NormalMoveCommand : MoveCommandBase
{
    public override async UniTask DoAsync(Transform target, MoveInfo moveInfo, CancellationToken token)
    {
        if (moveInfo?.TargetPosition.HasValue == false || moveInfo?.TargetRotation.HasValue == false)
        {
            return;
        }
        await CubeMoveUtil.MoveFlowAsync(target, moveInfo.TargetPosition.Value, moveInfo.TargetRotation.Value, moveInfo.MoveDuration, token);
    }

    public override async UniTask UndoAsync(Transform target, MoveInfo moveInfo, CancellationToken token)
    {
        await CubeMoveUtil.MoveFlowAsync(target, moveInfo.OriginPosition, moveInfo.OriginRotation, moveInfo.MoveDuration, token);
    }
}
