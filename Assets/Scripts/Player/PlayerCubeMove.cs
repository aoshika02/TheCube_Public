using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;

public partial class PlayerCube : SingletonMonoBehaviour<PlayerCube>
{
    // 一回あたりの回転角度
    [SerializeField] private float _rotateValue = 90f;
    [SerializeField] private float _unRotateValue = 30f;

    public Vector3 PlayerMovedPos(Vector2Int moveDir)
    {
        return CubeMoveUtil.GetNextPos(_rotateParent, moveDir, _moveDistance);
    }

    public async UniTask MoveYAsync(float targetPosY, float duration = 0.5f)
    {
        if (_isMoving)
        {
            Debug.Log("現在移動中です");
            return;
        }
        _isMoving = true;
        await _rotateParent.transform
            .DOMoveY(targetPosY, duration)
            .SetEase(Ease.Linear)
            .SetLink(gameObject);
        _isMoving = false;
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    /// <param name="moveDir">移動方向</param>
    /// <param name="detachment">離別を行うか</param>
    /// <param name="moveTime">Tweenのduration</param>
    /// <returns></returns>
    public async UniTask<MoveResult> MoveAsync(Vector2Int moveDir, bool detachment, CancellationToken token, DirectionType targetType = DirectionType.None, bool isInGame = true, float moveTime = 0.5f)
    {
        if (_isMoving)
        {
            Debug.Log("現在移動中です");
            return new(false, DirectionType.None);
        }
        _isMoving = true;

        Vector3 moveTarget = CubeMoveUtil.GetNextPos(_rotateParent, moveDir, _moveDistance);
        Quaternion targetRot = CubeMoveUtil.GetNextQuaternion(_rotateParent, moveDir, _rotateValue);

        if (isInGame == false)
        {
            await CubeMoveUtil.MoveFlowAsync(_rotateParent, moveTarget, targetRot, token, moveTime);
            _isMoving = false;
            return new(true, DirectionType.None);
        }

        DirectionType nextBottomDir = DirectionUtil.GetBottomDirection(targetRot);

        bool isReverse = _detachmentTypes.Contains(nextBottomDir) && nextBottomDir == targetType;
        bool isMoving = !_detachmentTypes.Contains(nextBottomDir) && targetType == DirectionType.None;

        if (isReverse == false && isMoving == false)
        {
            await UnMoveFlow(moveDir, detachment, token, moveTime);
            //移動不可
            _isMoving = false;
            return new(false, DirectionType.None);
        }

        DirectionType detachmentType = DirectionType.None;

        //平常移動
        if (isMoving)
        {
            if (detachment)
            {
                Detachment(_unRotateParent, _currentBottomDir);
                detachmentType = _currentBottomDir;
                _detachmentTypes.Add(_currentBottomDir);
                _moveDatas.Add(new MoveData(_currentBottomDir, moveDir, moveTarget, targetRot, TileType.Normal));
                ChangeCubeBaseColor(0, _lastAttachmentColor);
                ChangeCubeBaseColor(1, _detachmentColor);
            }
            else
            {
                _moveDatas.Add(new MoveData(DirectionType.None, moveDir, moveTarget, targetRot, TileType.Skip));
            }
            _detachmentFlags.Add(detachment);
            // 移動と回転のフローを実行
            await CubeMoveUtil.MoveFlowAsync(_rotateParent, moveTarget, targetRot, token, moveTime);
            _lastDir = moveDir;
        }
        // 戻り移動
        else if (isReverse)
        {
            await CubeMoveUtil.MoveFlowAsync(_rotateParent, moveTarget, targetRot, token, moveTime);
            if (_detachmentFlags.Count > 0 && _detachmentFlags[_detachmentFlags.Count - 1])
            {
                ChangeCubeBaseColor(0, _defaultColor);
                ChangeCubeBaseColor(1, _lastAttachmentColor);
                ChangeCubeBaseColor(2, _detachmentColor);
                Detachment(_rotateParent, nextBottomDir);
                detachmentType = nextBottomDir;
                _detachmentFlags.RemoveAt(_detachmentFlags.Count - 1);
            }
            if (_detachmentTypes.Contains(nextBottomDir))
                _detachmentTypes.Remove(nextBottomDir);

            if (_moveDatas.Count > 0)
                _moveDatas.RemoveAt(_moveDatas.Count - 1);
            if (_moveDatas.Count > 0)
            {
                _lastDir = _moveDatas[_moveDatas.Count - 1].MoveDir;
            }
            else
            {
                _lastDir = Vector2Int.zero;
            }
        }
        else
        {
            _isMoving = false;
            return new(false, DirectionType.None);
        }
        //完了後に下面情報を更新
        _currentBottomDir = nextBottomDir;
        _isMoving = false;
        return new(true, detachmentType);
    }

    public async UniTask UnMoveFlow(Vector2Int moveDir, bool detachment, CancellationToken token, float moveTime = 0.5f)
    {

        if (detachment) Detachment(_unRotateParent, _currentBottomDir);

        await CubeMoveUtil.MoveFlowAsync(
            _rotateParent,
            CubeMoveUtil.GetNextPos(_rotateParent, moveDir, _moveDistance / 2),
            CubeMoveUtil.GetNextQuaternion(_rotateParent, moveDir, _unRotateValue),
            token,
            moveTime);

        await CubeMoveUtil.MoveFlowAsync(
            _rotateParent,
            CubeMoveUtil.GetNextPos(_rotateParent, moveDir, -_moveDistance / 2),
            CubeMoveUtil.GetNextQuaternion(_rotateParent, moveDir, -_unRotateValue),
            token,
            moveTime);

        if (detachment) Detachment(_rotateParent, _currentBottomDir);
    }

    /// <summary>
    /// 移動可能かどうかを判定する
    /// </summary>
    /// <param name="moveDir"></param>
    /// <returns></returns>
    public bool IsMovable(Vector2Int moveDir)
    {
        Quaternion targetRot = CubeMoveUtil.GetNextQuaternion(_rotateParent, moveDir, _rotateValue);
        DirectionType nextBottomDir = DirectionUtil.GetBottomDirection(targetRot);
        bool isReverse = moveDir == -_lastDir;
        return !(_detachmentTypes.Contains(nextBottomDir) && isReverse == false);
    }

    public async UniTask PlayerCubeDoPath(Vector3[] path, float delay = 0.25f, float duration = 1f)
    {
        await _rotateParent.transform
           .DOPath(path, duration)
           .SetEase(Ease.InOutSine)
           .ToUniTask();
    }

}
