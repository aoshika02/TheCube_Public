using UnityEngine;

public static class TileTypeExtension
{
    public static bool IsArrow(this TileType tileType)
    {
        return tileType == TileType.UpperArrow || tileType == TileType.LeftArrow || tileType == TileType.DownArrow || tileType == TileType.RightArrow;
    }

    public static bool IsJump(this TileType tileType)
    {
        return tileType == TileType.JumpUp || tileType == TileType.JumpDown || tileType == TileType.JumpLeft || tileType == TileType.JumpRight;
    }

    public static Vector2Int ToVector2Int(this TileType tileType) 
    {
        return tileType switch
        {
            TileType.UpperArrow => Vector2Int.up,
            TileType.DownArrow => Vector2Int.down,
            TileType.LeftArrow => Vector2Int.left,
            TileType.RightArrow => Vector2Int.right,

            TileType.JumpUp => Vector2Int.up,
            TileType.JumpDown => Vector2Int.down,
            TileType.JumpLeft => Vector2Int.left,
            TileType.JumpRight => Vector2Int.right,

            _ => Vector2Int.zero
        };
    }
}