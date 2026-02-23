using System.Collections.Generic;
using UnityEngine;

public static class DirectionUtil
{
    public static DirectionType GetBottomDirection(Quaternion rotation)
    {
        Vector3 down = Vector3.down;

        var dirs = new Dictionary<DirectionType, Vector3>()
        {
            { DirectionType.Top,     rotation * Vector3.up },
            { DirectionType.Bottom,  rotation * Vector3.down },
            { DirectionType.Front,   rotation * Vector3.forward },
            { DirectionType.Back,    rotation * Vector3.back },
            { DirectionType.Right,   rotation * Vector3.right },
            { DirectionType.Left,    rotation * Vector3.left },
        };

        float maxDot = -999f;
        DirectionType result = DirectionType.None;

        foreach (var kvp in dirs)
        {
            float dot = Vector3.Dot(kvp.Value.normalized, down);

            if (dot > maxDot)
            {
                maxDot = dot;
                result = kvp.Key;
            }
        }

        return result;
    }

}
