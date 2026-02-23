using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public partial class PlayerCube : SingletonMonoBehaviour<PlayerCube>
{
    private TileType[] _ignoreTypes = new TileType[]
    {
        TileType.UpperArrow,
        TileType.JumpUp,
        TileType.Warp };

    public async UniTask ReverseMoveAsync(CancellationToken token, float moveTime = 0.5f)
    {
        if (_isMoving) return;
        _isMoving = true;
        List<DirectionType> directionTypes = _moveDatas.Select(x => x.DetachmentType).ToList();
        List<DirectionType> detachTypesCopy = new List<DirectionType>();
        for (int i = 0; i < _moveDatas.Count; i++)
        {
            TileType tileType = _moveDatas[i].TileType;
            DirectionType currentDir = DirectionUtil.GetBottomDirection(_reverseRotateParent.rotation);
            DirectionType nextDir = DirectionUtil.GetBottomDirection(_moveDatas[i].TargetRotation);

            if (directionTypes.Contains(currentDir) && !_ignoreTypes.Contains(tileType))
            {
                Detachment(_reverseRotateParent, currentDir);

                ChangeCubeBaseColor(currentDir, _defaultColor);
                if (i != _moveDatas.Count - 1 && i != 0)
                    ChangeCubeBaseColor(nextDir, _lastAttachmentColor);

                detachTypesCopy.Add(currentDir);
            }

            switch (tileType)
            {
                case TileType.Normal:
                case TileType.Skip:
                    await CubeMoveUtil.MoveFlowAsync(_reverseRotateParent, _moveDatas[i].TargetPosition, _moveDatas[i].TargetRotation, token, moveTime);
                    break;
                case TileType.UpperArrow:
                    await CubeMoveUtil.AccelMoveAsync(_reverseRotateParent, _moveDatas[i].TargetPosition, moveTime);
                    break;
                case TileType.JumpUp:
                    await CubeMoveUtil.JumpMoveAsync(_reverseRotateParent, _moveDatas[i].TargetPosition, moveTime: moveTime);
                    break;
                case TileType.Warp:
                    await _reverseRotateParent.DOScale(Vector3.zero, moveTime / 2).SetEase(Ease.InSine).ToUniTask();
                    _reverseRotateParent.position = _moveDatas[i].TargetPosition;
                    await _reverseRotateParent.DOScale(Vector3.one, moveTime / 2).SetEase(Ease.OutSine).ToUniTask();
                    break;
                default:
                    break;
            }

        }

        foreach (var directionType in detachTypesCopy)
        {
            var bottomObj = GetCubeTile(directionType);
            if (bottomObj != null)
            {
                Detachment(bottomObj.CubeTile.gameObject, _rotateParent);
                _detachmentTypes.Remove(directionType);
                ChangeCubeBaseColor(directionType, _defaultColor);
            }
        }
        Clear();

        _isMoving = false;
    }

    public async UniTask AccelMoveFlow(Vector3 moveTarget, float moveTime = 0.5f)
    {
        await CubeMoveUtil.AccelMoveAsync(_rotateParent, moveTarget, moveTime);
        _moveDatas.Add(new MoveData(_currentBottomDir, Vector2Int.zero, moveTarget, _rotateParent.rotation, TileType.UpperArrow));
        ChangeCubeBaseColor(0, _detachmentColor);
    }

    public async UniTask JumpFlow(Vector3 moveTarget, float moveTime = 0.25f)
    {
        await CubeMoveUtil.JumpMoveAsync(_rotateParent, moveTarget, moveTime: moveTime);
        _moveDatas.Add(new MoveData(DirectionType.None, Vector2Int.zero, moveTarget, _rotateParent.rotation, TileType.JumpUp));
    }

    public async UniTask WarpFlow(Vector3 moveTarget, float moveTime = 0.5f)
    {
        await _rotateParent.DOScale(Vector3.zero, moveTime / 2).SetEase(Ease.InSine).ToUniTask();
        SetPos(moveTarget);
        await _rotateParent.DOScale(Vector3.one, moveTime / 2).SetEase(Ease.OutSine).ToUniTask();
        _moveDatas.Add(new MoveData(DirectionType.None, Vector2Int.zero, moveTarget, _rotateParent.rotation, TileType.Warp));
    }
}
