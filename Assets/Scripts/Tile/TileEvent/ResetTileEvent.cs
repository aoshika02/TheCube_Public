public class ResetTileEvent : TileEventBase
{
    public ResetTileEvent(PlayerCube playerCube,TileManager tileManager) : base(playerCube, tileManager)
    {

    }

    public override MoveInfo OnCommandFinish(MoveInfo moveInfo)
    {
        _tileManager.ClearDetachmentType();
        return base.OnCommandFinish(moveInfo);
    }
}
