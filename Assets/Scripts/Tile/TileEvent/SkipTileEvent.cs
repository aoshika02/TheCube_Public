public class SkipTileEvent : TileEventBase
{
    public SkipTileEvent(PlayerCube playerCube, TileManager tileManager) : base(playerCube, tileManager)
    {

    }

    public override MoveInfo OnEnter(ParentType parentType, TileType tileType, DirectionType detachmentType)
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

    public override DirectionType OnLeave(ParentType parentType)
    {
        return DirectionType.None;
    }

    public override DirectionType OnReverse(ParentType parentType, DirectionType nextBottomDir)
    {
        return nextBottomDir;
    }
}
