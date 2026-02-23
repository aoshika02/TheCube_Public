using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;

public static class CubeMoveUtil
{
    public static async UniTask MoveFlowAsync(Transform target, Vector3 moveTarget, Quaternion targetRot, CancellationToken token, float moveTime = 0.5f)
    {
        //DOTweenで移動と回転を同時に行う
        var moveTask = target.DOMove(moveTarget, moveTime).SetEase(Ease.Linear).ToUniTask();
        var rotateTask = target.DORotateQuaternion(targetRot, moveTime).SetEase(Ease.Linear).ToUniTask();

        await UniTask.WhenAll(moveTask, rotateTask);
    }

    public static async UniTask AccelMoveAsync(Transform target, Vector3 moveTarget, float moveTime = 0.5f) 
    {
        var distance = Vector3.Distance(target.position, moveTarget);
        await target.DOMove(moveTarget, moveTime * distance).SetEase(Ease.InSine).ToUniTask();
    }

    public static async UniTask JumpMoveAsync(Transform target, Vector3 moveTarget,float jumpHight = 1f, float moveTime = 0.5f)
    {
        var distance = Vector3.Distance(target.position, moveTarget);
        await target.DOJump(moveTarget, jumpHight, 1, moveTime * distance).SetEase(Ease.InSine).ToUniTask();
    }

    public static Vector3 GetNextPos(Transform target, Vector2 moveDir, float moveDistanceValue = 1)
    {
        // 移動先のワールド位置
        return new Vector3(moveDir.x * moveDistanceValue, 0, moveDir.y * moveDistanceValue) + target.position;
    }

    public static Quaternion GetNextQuaternion(Transform target, Vector2 moveDir, float rotateValue = 90f)
    {
        // 回転量
        Vector3 deltaEuler = new Vector3(moveDir.y * rotateValue, 0f, -moveDir.x * rotateValue);

        // 現在の回転を基準にQuaternionを作る
        Quaternion currentRot = target.rotation;
        Quaternion deltaQuat = Quaternion.Euler(deltaEuler);
        return deltaQuat * currentRot;
    }
}
