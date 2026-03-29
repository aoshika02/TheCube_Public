using UnityEngine;

public class AccelTileEvent : TileEventBase
{
    public AccelTileEvent(PlayerCube playerCube, TileManager tileManager) : base(playerCube, tileManager)
    {

    }

    public override MoveInfo OnEnter(ParentType parentType, TileType tileType, DirectionType detachmentType)
    {
        var parent = _playerCube.GetParent(parentType);
        var moveDir = tileType.ToVector2Int();
        if (TryGetCornerPos(parent.position, moveDir, out var cornerPos))
        {
            var cornerTileObj = _tileManager.GetTileObj(cornerPos);
            return new MoveInfo
            (
                parent.position,
                parent.rotation,
                cornerPos,
                null,
                tileType,
                cornerTileObj != null ? cornerTileObj.TileType : null,
                detachmentType,
                parentType
            );
        }
        return null;
    }

    private bool TryGetCornerPos(Vector3 pos, Vector2Int moveDir, out Vector3 target)
    {
        target = Vector3.zero;
        if (moveDir == Vector2Int.zero) return false;

        float tileSize = _tileManager.TileSize;
        Vector3 checkPos = pos;

        while (true)
        {
            Vector3 nextPos = checkPos + new Vector3(moveDir.x * tileSize, 0, moveDir.y * tileSize);
            if (_tileManager.PosToID(nextPos) == -1) break;
            if (_tileManager.GetTileObj(nextPos) == null) break;
            checkPos = nextPos;
        }

        if (checkPos == pos) return false;

        TileObj cornerTile = _tileManager.GetTileObj(checkPos);
        if (cornerTile == null) return false;

        target = cornerTile.transform.position;
        target.y = 0;
        return true;
    }
}
