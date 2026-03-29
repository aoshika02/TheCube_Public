using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public interface IMoveCommand
{
    UniTask DoAsync(Transform target, MoveInfo moveInfo, CancellationToken token);
    UniTask UndoAsync(Transform target, MoveInfo moveInfo, CancellationToken token);
    UniTask LeaveAsync(Transform target, Vector3 moveTarget, Quaternion targetRot, float moveTime, CancellationToken token);
}

public record MoveInfo
{
    public readonly Vector3 OriginPosition;
    public readonly Quaternion OriginRotation;
    public readonly Vector3? TargetPosition;
    public readonly Quaternion? TargetRotation;
    public readonly TileType CurrentTileType;
    public readonly TileType? TargetTileType;
    public readonly DirectionType DetachmentType;
    public readonly ParentType ParentType;
    public readonly float MoveDuration;

    public MoveInfo(
              Vector3 originPosition,
              Quaternion originRotation,
              Vector3? targetPosition,
              Quaternion? targetRotation,
              TileType currentTileType,
              TileType? targetTileType,
              DirectionType detachmentType,
              ParentType parentType,
              float moveDuration = 0.5f)
    {
        OriginPosition = originPosition;
        OriginRotation = originRotation;
        TargetPosition = targetPosition;
        TargetRotation = targetRotation;
        CurrentTileType = currentTileType;
        TargetTileType = targetTileType;
        DetachmentType = detachmentType;
        ParentType = parentType;
        MoveDuration = moveDuration;
    } 
}
