public class EmptyTileEvent : TileEventBase
{
    public EmptyTileEvent(PlayerCube playerCube, TileManager tileManager) : base(playerCube, tileManager)
    {

    }

    public override MoveInfo OnCommandFinish(MoveInfo moveInfo)
    {
        return null;
    }

    public override MoveInfo OnEnter(ParentType parentType, TileType tileType, DirectionType detachmentType)
    {
        return null;
    }

    public override DirectionType OnLeave(ParentType parentType)
    {
        return DirectionType.None;
    }
}
