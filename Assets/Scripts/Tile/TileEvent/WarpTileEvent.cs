using System.Linq;
using UnityEngine;

public class WarpTileEvent : TileEventBase
{
    public WarpTileEvent(PlayerCube playerCube, TileManager tileManager) : base(playerCube, tileManager)
    {
    }

    public override MoveInfo OnEnter(ParentType parentType, TileType tileType, DirectionType detachmentType)
    {
        var parent = _playerCube.GetParent(parentType);
        if (TryGetWarpPos(parent.position, tileType, out var warpPos))
        {
            return new MoveInfo
            (
                parent.position,
                parent.rotation,
                warpPos,
                null,
                tileType,
                TileType.Normal,
                detachmentType,
                parentType
            );
        }
        return null;
    }

    private bool TryGetWarpPos(Vector3 currentPos, TileType tileType, out Vector3 warpPos)
    {
        int currentId = _tileManager.PosToID(currentPos);
        var target = _tileManager.GetActiveTileObjs()
            .FirstOrDefault(t => _tileManager.PosToID(t.transform.position) != currentId
                                 && t.TileType == tileType);
        if (target != null)
        {
            warpPos = target.transform.position;
            warpPos.y = 0;
            return true;
        }
        warpPos = Vector3.zero;
        return false;
    }
}
