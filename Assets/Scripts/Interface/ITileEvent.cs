public interface ITileEvent
{
    MoveInfo OnCommandFinish(MoveInfo moveInfo);
    MoveInfo OnEnter(ParentType parentType, TileType tileType, DirectionType detachmentType);
    DirectionType OnLeave(ParentType parentType);
    DirectionType OnReverse(ParentType parentType, DirectionType nextBottomDir);
}