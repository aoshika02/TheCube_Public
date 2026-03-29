using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class EmptyMoveCommand : MoveCommandBase
{
    public override UniTask DoAsync(Transform target, MoveInfo moveInfo, CancellationToken token)
    {
        return UniTask.CompletedTask;
    }

    public override UniTask UndoAsync(Transform target, MoveInfo moveInfo, CancellationToken token)
    {
        return UniTask.CompletedTask;
    }
}
