public class JumpTileEvent : TileEventBase
{
    private readonly PlayerCamera _playerCameraMove;
    private readonly int _jumpDistance = 2;

    public JumpTileEvent(PlayerCube playerCube, TileManager tileManager, PlayerCamera playerCameraMove, int jumpDistance) : base(playerCube, tileManager)
    {
        _playerCameraMove = playerCameraMove;
        _jumpDistance = jumpDistance;
    }

    public override MoveInfo OnEnter(ParentType parentType, TileType tileType, DirectionType detachmentType)
    {
        var moveDir = tileType.ToVector2Int();
        var parent = _playerCube.GetParent(parentType);
        var moveTarget = CubeMoveUtil.GetNextPos(parent, moveDir * _jumpDistance);
        var targetTileObj = _tileManager.GetTileObj(moveTarget);
        if (targetTileObj == null)
        {
            _playerCameraMove.SetFollow(false);
        }

        return new MoveInfo
        (
            parent.position,
            parent.rotation,
            moveTarget,
            null,
            tileType,
            targetTileObj != null ? targetTileObj.TileType : null,
            detachmentType,
            parentType
        );
    }
}
