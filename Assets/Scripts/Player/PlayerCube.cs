using System.Collections.Generic;
using UnityEngine;

public partial class PlayerCube : SingletonMonoBehaviour<PlayerCube>
{
    [SerializeField] private Transform _rotateParent;
    [SerializeField] private Transform _unRotateParent;
    [SerializeField] private Transform _reverseRotateParent;
    [SerializeField] private DirectionType _currentBottomDir = DirectionType.Bottom;
    // 一回あたりの移動距離
    [SerializeField] private float _moveDistance = 1f;
    private List<DirectionType> _detachmentTypes = new List<DirectionType>();
    private List<MoveData> _moveDatas = new List<MoveData>();
    private List<bool> _detachmentFlags = new List<bool>();
    private Vector2Int _lastDir = Vector2Int.zero;
    private bool _isMoving = false;
    
    public Vector3 GetPlayerPos()
    {
        return _rotateParent.position;
    }

    public void SetPos(Vector3 pos)
    {
        _rotateParent.position = pos;
    }

    #region リセット関係メソッド
    public void Clear(Vector3 pos, Quaternion rot, bool isResetBases = false)
    {
        // 位置と回転をリセット
        _rotateParent.position = pos;
        _rotateParent.rotation = rot;
        _reverseRotateParent.position = pos;
        _reverseRotateParent.rotation = rot;

        if (isResetBases)
        {
            ResetAllCubeTile();
        }
        Clear();
    }

    public void Clear()
    {
        // 移動履歴をリセット
        _moveDatas.Clear();
        _detachmentFlags.Clear();
        _detachmentTypes.Clear();
        _lastDir = Vector2Int.zero;
        _currentBottomDir = DirectionUtil.GetBottomDirection(_rotateParent.rotation);
    }
    #endregion
}


public class MoveData
{
    public DirectionType DetachmentType;
    public Vector2Int MoveDir;
    public Vector3 TargetPosition;
    public Quaternion TargetRotation;
    public TileType TileType;

    public MoveData(DirectionType directionType, Vector2Int moveDir, Vector3 targetPos, Quaternion targetRot, TileType tileType)
    {
        DetachmentType = directionType;
        MoveDir = moveDir;
        TargetPosition = targetPos;
        TargetRotation = targetRot;
        TileType = tileType;
    }
}

public record MoveResult
{
    public bool IsSuccess;
    public DirectionType DetachmentType;

    public MoveResult(bool isSuccess, DirectionType detachmentType)
    {
        IsSuccess = isSuccess;
        DetachmentType = detachmentType;
    }
}