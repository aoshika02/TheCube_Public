using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;

public class WarpMoveCommand : MoveCommandBase
{
    public override async UniTask DoAsync(Transform target, MoveInfo moveInfo, CancellationToken token)
    {
        if (moveInfo?.TargetPosition.HasValue == false)
        {
            return;
        }
        await target.DOScale(Vector3.zero, moveInfo.MoveDuration / 2).SetEase(Ease.InSine).ToUniTask(cancellationToken: token);
        target.position = moveInfo.TargetPosition.Value;
        await target.DOScale(Vector3.one, moveInfo.MoveDuration / 2).SetEase(Ease.OutSine).ToUniTask(cancellationToken: token);
    }

    public override async UniTask UndoAsync(Transform target, MoveInfo moveInfo, CancellationToken token)
    {
        await target.DOScale(Vector3.zero, moveInfo.MoveDuration / 2).SetEase(Ease.InSine).ToUniTask(cancellationToken: token);
        target.position = moveInfo.OriginPosition;
        await target.DOScale(Vector3.one, moveInfo.MoveDuration / 2).SetEase(Ease.OutSine).ToUniTask(cancellationToken: token);
    }
}
