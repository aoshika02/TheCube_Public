using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class TileManager : MonoBehaviour
{
    private Dictionary<int, TileObj> _activeTilesDic = new Dictionary<int, TileObj>();
    private Dictionary<int, DirectionType> _detachmentDic = new Dictionary<int, DirectionType>();

    public void AddActiveTile(int id, TileObj tileObj)
    {
        if (_activeTilesDic.TryAdd(id, tileObj) == false)
        {
            Debug.LogError($"タイルの辞書登録に失敗しました -> id:{id}");
        }
    }

    public bool GetActiveTileData(int id, out TileObj tileObj)
    {
        if (_activeTilesDic.TryGetValue(id, out tileObj))
        {
            return true;
        }
        return false;
    }

    public List<TileObj> GetActiveTileObjs() =>
        _activeTilesDic.Values.Where(x => x != null).ToList();

    public void SetDetachmentType(Vector3 pos, DirectionType detachmentType)
    {
        int id = PosToID(pos);

        var existingKey = _detachmentDic
            .Where(x => x.Value == detachmentType)
            .Select(x => (int?)x.Key)
            .FirstOrDefault();

        if (existingKey.HasValue)
        {
            _detachmentDic.Remove(existingKey.Value);
            return;
        }

        if (detachmentType == DirectionType.None)
        {
            _detachmentDic.Remove(id);
            return;
        }

        _detachmentDic[id] = detachmentType;
    }

    public void ClearDetachmentType()
    {
        _detachmentDic.Clear();
    }

    public DirectionType GetDetachmentType(Vector3 pos)
    {
        int id = PosToID(pos);
        return _detachmentDic.TryGetValue(id, out var dir) ? dir : DirectionType.None;
    }

    public void ClearActiveTile()
    {
        _activeTilesDic.Clear();
        _detachmentDic.Clear();
    }
}