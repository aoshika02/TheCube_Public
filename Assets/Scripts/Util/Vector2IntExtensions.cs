using UnityEngine;

namespace Extension {
    public static class Vector2IntExtensions
    {
        public static Vector2Int Arrow2Vector2Int(this ArrowType arrowType)
        {
            return arrowType switch
            {
                ArrowType.Zero => Vector2Int.zero,
                ArrowType.Up => Vector2Int.up,
                ArrowType.Down => Vector2Int.down,
                ArrowType.Left => Vector2Int.left,
                ArrowType.Right => Vector2Int.right,
                _ => Vector2Int.zero
            };
        }
    }
    public enum ArrowType
    {
        Zero,
        Up,
        Down,
        Left,
        Right
    }
}
