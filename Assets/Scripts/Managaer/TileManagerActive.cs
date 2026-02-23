using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class TileManager : SingletonMonoBehaviour<TileManager>
{
    private Dictionary<int, TileActiveData> _activeTilesDic = new Dictionary<int, TileActiveData>();

    public void AddActiveTile(int id, DirectionType directionType, TileObj tileObj)
    {
        if (_activeTilesDic.TryAdd(id, new TileActiveData(directionType, tileObj)) == false)
        {
            Debug.LogError($"タイルの辞書登録に失敗しました -> id:{id},{directionType}");
        }
    }

    public bool GetActiveTileData(int id, out TileActiveData tileActiveData)
    {
        if (_activeTilesDic.TryGetValue(id, out tileActiveData))
        {
            return true;
        }
        return false;
    }

    public bool GetWarpPos(Vector3 pos, TileType tileType, out Vector3 warpPos)
    {
        int id = PosToID(pos);

        foreach (var pair in _activeTilesDic)
        {
            if (pair.Key == id) continue;

            var tile = pair.Value?.TileObj;
            if (tile != null && tile.TileType == tileType)
            {
                warpPos = tile.transform.position;
                warpPos.y = 0;
                return true;
            }
        }
        warpPos = Vector3.zero;
        return false;
    }

    public void SetDetachmentType(Vector3 pos, DirectionType detachmentType)
    {
        int id = PosToID(pos);

        TileActiveData activeData = _activeTilesDic
            .Select(x => x.Value).ToList()
            .Where(y => y.DetachmentType != DirectionType.None)
            .FirstOrDefault(z => z.DetachmentType == detachmentType);

        if (activeData != null)
        {
            activeData.DetachmentType = DirectionType.None;
            return;
        }

        if (_activeTilesDic.TryGetValue(id, out TileActiveData tileActiveData))
        {
            if (tileActiveData == null) return;
            tileActiveData.DetachmentType = detachmentType;
        }
    }

    public void ClearDetachmentType()
    {
        foreach (var tileActiveData in _activeTilesDic.Values.Where(x => x.DetachmentType != DirectionType.None))
        {
            tileActiveData.DetachmentType = DirectionType.None;
        }
    }

    public DirectionType GetDetachmentType(Vector3 pos)
    {
        int id = PosToID(pos);
        if (_activeTilesDic.TryGetValue(id, out TileActiveData tileActiveData))
        {
            if (tileActiveData == null) return DirectionType.None;
            return tileActiveData.DetachmentType;
        }
        return DirectionType.None;
    }

}

public class TileActiveData
{
    public DirectionType DetachmentType;
    public TileObj TileObj;

    public TileActiveData(DirectionType detachmentType, TileObj tileObj)
    {
        DetachmentType = detachmentType;
        TileObj = tileObj;
    }
}