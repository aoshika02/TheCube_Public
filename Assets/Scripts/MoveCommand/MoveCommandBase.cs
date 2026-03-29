using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public abstract class MoveCommandBase : IMoveCommand
{
    public virtual UniTask DoAsync(Transform target, MoveInfo moveInfo, CancellationToken token)
    {
        return UniTask.CompletedTask;
    }

    public virtual UniTask UndoAsync(Transform target, MoveInfo moveInfo, CancellationToken token)
    {
        return UniTask.CompletedTask;
    }

    public virtual UniTask LeaveAsync(Transform target, Vector3 moveTarget, Quaternion targetRot, float moveTime, CancellationToken token)
    {
        return CubeMoveUtil.MoveFlowAsync(target, moveTarget, targetRot, moveTime, token);
    }

}
