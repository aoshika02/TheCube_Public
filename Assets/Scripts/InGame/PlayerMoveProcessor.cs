using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class PlayerMoveProcessor
{
    private TileManager _tileManager;
    private PlayerCube _playerCube;
    private TileEventLocator _tileEventLocator;
    private MoveCommandLocator _moveCommandLocator;
    private readonly MoveHistory _moveHistory = new();
    private DirectionType _currentBottomDir = DirectionType.Bottom;
    private const float UN_ROTATE_VALUE = 30f;
    private bool _isCleared = false;

    private TileType[] _ignoreTypes = new TileType[]
    {
        TileType.UpperArrow,
        TileType.JumpUp,
        TileType.Warp,
        TileType.Skip
    };

    public PlayerMoveProcessor(
        TileManager tileManager,
        PlayerCube playerCube,
        TileEventLocator tileEventLocator,
        MoveCommandLocator moveCommandLocator)
    {
        _tileManager = tileManager;
        _playerCube = playerCube;
        _tileEventLocator = tileEventLocator;
        _moveCommandLocator = moveCommandLocator;
    }

    private List<DirectionType> GetDetachedFaceDirs()
    {
        return _moveHistory.CurrentSessionInfos
            .Where(x => x.CurrentTileType != TileType.Skip && x.DetachmentType != DirectionType.None)
            .Select(x => x.DetachmentType)
            .Distinct()
            .ToList();
    }

    public async UniTask<ResultType> OnMove(Vector2Int moveDir, CancellationToken token)
    {
        var cubeTransform = _playerCube.GetParent(ParentType.Rotate);
        var currentPos = cubeTransform.position;
        var movedPos = CubeMoveUtil.GetNextPos(cubeTransform, moveDir);
        var currentTileType = _tileManager.GetTileType(currentPos);
        var targetTileType = _tileManager.GetTileType(movedPos);
        Quaternion targetRot = CubeMoveUtil.GetNextQuaternion(cubeTransform, moveDir);
        if (targetTileType == TileType.None)
        {
            await UnMoveFlow(moveDir, token: token);
            return ResultType.None;
        }

        var targetType = _tileManager.GetDetachmentType(movedPos);
        var moveType = MoveableCheck(moveDir, targetType);

        DirectionType detachmentDir = DirectionType.None;
        IMoveCommand moveCommand = null;
        ITileEvent tileEvent = null;

        switch (moveType)
        {
            case MoveType.Move:
                var originRot = cubeTransform.rotation;
                tileEvent = _tileEventLocator.Resolve(currentTileType);
                detachmentDir = tileEvent.OnLeave(ParentType.Rotate);
                if (currentTileType != TileType.Skip)
                {
                    var prevDirs = GetDetachedFaceDirs();
                    if (prevDirs.Count > 0)
                        _playerCube.ChangeCubeBaseColor(prevDirs[prevDirs.Count - 1], ColorType.Detachment);
                }
                moveCommand = _moveCommandLocator.Resolve(currentTileType);
                _tileManager.SetDetachmentType(currentPos, detachmentDir);
                await moveCommand.LeaveAsync(cubeTransform, movedPos, targetRot, 0.5f, token);
                _moveHistory.Add(new MoveInfo(currentPos, originRot, movedPos, targetRot, currentTileType, targetTileType, detachmentDir, ParentType.Rotate));
                if (currentTileType != TileType.Skip)
                    _playerCube.ChangeCubeBaseColor(detachmentDir, ColorType.LastAttachment);
                break;
            case MoveType.ReverseMove:
                moveCommand = _moveCommandLocator.Resolve(currentTileType);
                tileEvent = _tileEventLocator.Resolve(currentTileType);
                _tileManager.SetDetachmentType(movedPos, DirectionType.None);
                await moveCommand.LeaveAsync(cubeTransform, movedPos, targetRot, 0.5f, token);
                SetCurrentBottomDir(DirectionUtil.GetBottomDirection(targetRot));
                tileEvent.OnReverse(ParentType.Rotate, _playerCube.CurrentBottomDir);
                var lastInfo = _moveHistory.GetFromEnd(0);
                if (lastInfo != null && lastInfo.CurrentTileType != TileType.Skip)
                {
                    var dirs = GetDetachedFaceDirs();
                    if (dirs.Count > 0)
                        _playerCube.ChangeCubeBaseColor(dirs[dirs.Count - 1], ColorType.Default);
                    if (dirs.Count > 1)
                        _playerCube.ChangeCubeBaseColor(dirs[dirs.Count - 2], ColorType.LastAttachment);
                }
                RemoveInfo();
                break;
            default:
                await UnMoveFlow(moveDir, token: token);
                return ResultType.None;
        }
        SetCurrentBottomDir(DirectionUtil.GetBottomDirection(targetRot));

        var result = await TileEvent(ParentType.Rotate, targetTileType, detachmentDir, token);

        switch (result)
        {
            case ResultType.Goal:
                _isCleared = true;
                return ResultType.Goal;
            case ResultType.Reset:
                return ResultType.Reset;
        }

        return ResultType.None;
    }

    private async UniTask<ResultType> TileEvent(ParentType parentType, TileType tileType, DirectionType detachmentType, CancellationToken token)
    {
        if (tileType == TileType.Goal)
        {
            return ResultType.Goal;
        }

        if (tileType == TileType.None || tileType == TileType.Normal || tileType == TileType.Skip)
        {
            return ResultType.None;
        }

        var tileEvent = _tileEventLocator.Resolve(tileType);
        parentType = tileType == TileType.Reset ? ParentType.ReverseRotate : parentType;
        var moveInfo = tileEvent.OnEnter(parentType, tileType, detachmentType);

        if (moveInfo == null)
        {
            if (tileType.IsJump())
            {
                return ResultType.Reset;
            }
            return ResultType.None;
        }

        if (tileType == TileType.Reset)
        {
            await ReverseMoveAsync(token);
            var finishedMovedInfo = tileEvent.OnCommandFinish(moveInfo);
            _moveHistory.Add(finishedMovedInfo);
            _moveHistory.StartNewSession();
            return ResultType.None;
        }

        var moveCommand = _moveCommandLocator.Resolve(tileType);
        var parent = _playerCube.GetParent(parentType);
        await moveCommand.DoAsync(parent, moveInfo, token);

        var finishedInfo = tileEvent.OnCommandFinish(moveInfo);
        _moveHistory.Add(finishedInfo);

        if (finishedInfo.TargetTileType.HasValue == false)
        {
            if (tileType.IsJump())
            {
                return ResultType.Reset;
            }
        }

        if (finishedInfo.TargetTileType != TileType.None && finishedInfo.TargetTileType != TileType.Normal)
        {
            var result = await TileEvent(parentType, finishedInfo.TargetTileType.Value, DirectionType.None, token);
            return result;
        }
        return ResultType.None;
    }

    public async UniTask ReverseMoveAsync(CancellationToken token = default)
    {
        var sessionInfos = _moveHistory.CurrentSessionInfos;
        List<DirectionType> detachTypesCopy = new List<DirectionType>();
        var reverseRotateParent = _playerCube.GetParent(ParentType.ReverseRotate);

        for (int i = 0; i < sessionInfos.Count; i++)
        {
            TileType tileType = sessionInfos[i].CurrentTileType;
            DirectionType detachmentDir = sessionInfos[i].DetachmentType;
            DirectionType nextDir = DirectionUtil.GetBottomDirection(sessionInfos[i].TargetRotation.Value);

            if (detachmentDir != DirectionType.None
                && !_ignoreTypes.Contains(tileType)
                && !detachTypesCopy.Contains(detachmentDir))
            {
                _playerCube.Detachment(ParentType.ReverseRotate, detachmentDir);
                _playerCube.GetCubeFace(detachmentDir)?.ResetTransform();

                _playerCube.ChangeCubeBaseColor(detachmentDir, ColorType.Default);
                if (i != sessionInfos.Count - 1 && i != 0)
                    _playerCube.ChangeCubeBaseColor(nextDir, ColorType.LastAttachment);

                detachTypesCopy.Add(detachmentDir);
            }

            if (tileType == TileType.None || tileType == TileType.Reset)
            {
                continue;
            }

            var moveCommand = _moveCommandLocator.Resolve(tileType);
            await moveCommand.DoAsync(reverseRotateParent, sessionInfos[i], token);
        }

        _playerCube.Detachments(detachTypesCopy);
        foreach (var directionType in detachTypesCopy)
        {
            _playerCube.ChangeCubeBaseColor(directionType, ColorType.Default);
        }
        _playerCube.ResetAllCubeTile();
    }

    public void RemoveInfo() => _moveHistory.RemoveLast();

    public void SetCurrentBottomDir(DirectionType directionType)
    {
        _currentBottomDir = directionType;
    }

    public async UniTask UnMoveFlow(Vector2Int moveDir, float moveTime = 0.5f, CancellationToken token = default)
    {
        var rotateParent = _playerCube.GetParent(ParentType.Rotate);
        _playerCube.Detachment(ParentType.UnRotate, _currentBottomDir);

        await CubeMoveUtil.UnMoveFlow(rotateParent, moveDir, unRotateValue: UN_ROTATE_VALUE, moveTime: moveTime, token: token);

        _playerCube.Detachment(ParentType.Rotate, _currentBottomDir);
    }

    public MoveType MoveableCheck(Vector2Int moveDir, DirectionType targetType = DirectionType.None)
    {
        var parent = _playerCube.GetParent(ParentType.Rotate);
        Quaternion targetRot = CubeMoveUtil.GetNextQuaternion(parent, moveDir);
        DirectionType nextBottomDir = DirectionUtil.GetBottomDirection(targetRot);
        var detachedDirs = GetDetachedFaceDirs();
        bool isReverse     = detachedDirs.Contains(nextBottomDir) && nextBottomDir == targetType;
        bool isSkipReverse = !detachedDirs.Contains(nextBottomDir) && targetType != DirectionType.None && nextBottomDir == targetType;
        bool isMovable     = !detachedDirs.Contains(nextBottomDir) && targetType == DirectionType.None;

        if (isMovable)
        {
            return MoveType.Move;
        }
        if (isReverse || isSkipReverse)
        {
            return MoveType.ReverseMove;
        }
        return MoveType.UnMove;
    }

    public void Reset()
    {
        _moveHistory.StartNewSession();
        _currentBottomDir = DirectionType.Bottom;
    }

    public void Initialize()
    {
        _moveHistory.Clear();
        _currentBottomDir = DirectionType.Bottom;
        _isCleared = false;
    }

    public bool IsCleared() => _isCleared;  
}
