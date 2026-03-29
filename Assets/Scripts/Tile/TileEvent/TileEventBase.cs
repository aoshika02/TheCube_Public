public abstract class TileEventBase : ITileEvent
{
    protected readonly PlayerCube _playerCube;
    protected readonly TileManager _tileManager;

    public TileEventBase(PlayerCube playerCube, TileManager tileManager)
    {
        _playerCube = playerCube;
        _tileManager = tileManager;
    }

    public virtual MoveInfo OnCommandFinish(MoveInfo moveInfo)
    {
        var parent = _playerCube.GetParent(moveInfo.ParentType);
        return new MoveInfo
        (
            moveInfo.OriginPosition,
            moveInfo.OriginRotation,
            parent.position,
            parent.rotation,
            moveInfo.CurrentTileType,
            moveInfo.TargetTileType,
            moveInfo.DetachmentType,
            moveInfo.ParentType,
            moveInfo.MoveDuration
        );
    }

    public virtual MoveInfo OnEnter(ParentType parentType, TileType tileType, DirectionType detachmentType)
    {
        var parent = _playerCube.GetParent(parentType);
        var targetTileObj = _tileManager.GetTileObj(parent.position);
        return new MoveInfo
        (
            parent.position,
            parent.rotation,
            null,
            null,
            tileType,
            targetTileObj != null ? targetTileObj.TileType : null,
            detachmentType,
            parentType
        );
    }

    public virtual DirectionType OnLeave(ParentType parentType)
    {
        _playerCube.Detachment(ParentType.UnRotate, _playerCube.CurrentBottomDir);
        return _playerCube.CurrentBottomDir;
    }

    public virtual DirectionType OnReverse(ParentType parentType, DirectionType nextBottomDir)
    {
        _playerCube.Detachment(parentType, nextBottomDir);
        return nextBottomDir;
    }
}
