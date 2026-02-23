using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public partial class TileManager : SingletonMonoBehaviour<TileManager>
{
    private TilePool _tilePool;
    [SerializeField] private int _gridSizeX;
    [SerializeField] private int _gridSizeY;
    [SerializeField] private float _tileSize = 1f;
    [SerializeField] private float _tilePosY = -0.575f;
    private List<List<TileType>> _stageData = new List<List<TileType>>();

    private DataSetLoader _dataSetLoader;

    public void Init(ReadOnlyCollection<ReadOnlyCollection<TileType>> tileTypes, UseType useType = UseType.Stage, bool startAnim = true, string layerName = "Default")
    {
        if (_tilePool == null) _tilePool = TilePool.Instance;
        if (tileTypes == null)
        {
            Debug.LogError("タイルタイプのリストがありません");
            return;
        }
        if (_dataSetLoader == null) _dataSetLoader = DataSetLoader.Instance;

        _gridSizeY = tileTypes.Count;
        _gridSizeX = 0;
        foreach (var type in tileTypes)
        {
            if (type == null) continue;
            if (type.Count > _gridSizeX)
                _gridSizeX = type.Count;
        }
        List<TileType> row = new List<TileType>();
        for (int j = 0; j < _gridSizeY; j++)
        {
            if (tileTypes[j] == null)
            {
                Debug.LogError($"タイルタイプのリストがありません -> index{j}");
                return;
            }
            for (int i = 0; i < _gridSizeX; i++)
            {
                if (tileTypes[j].Count <= i)
                {
                    row.Add(TileType.None);
                    continue;
                }
                var type = tileTypes[j][i];
                row.Add(type);
                if (type == TileType.None) continue;
                InitTile(j, i, type, useType, _gridSizeY, startAnim, layerName);
            }
            _stageData.Add(new List<TileType>(row));
            row.Clear();
        }

    }

    #region タイル取得

    public TileObj GetTileObj(int id)
    {
        if (GetActiveTileData(id, out TileActiveData tileActiveData))
        {
            if (tileActiveData == null) return null;
            return tileActiveData.TileObj;
        }
        return null;
    }

    public TileObj GetTileObj(Vector3 pos)
    {
        int id = PosToID(pos);
        return GetTileObj(id);
    }

    public TileType GetTileType(Vector3 pos)
    {
        var tileObj = GetTileObj(pos);
        if (tileObj == null)
        {
            return TileType.None;
        }
        return tileObj.TileType;
    }

    private bool GetNextTileObj(Vector3 pos, Vector2Int moveDir, out TileObj tileObj)
    {
        tileObj = GetTileObj(new Vector3(
            pos.x + _tileSize * moveDir.x,
            pos.y,
            pos.z + _tileSize * moveDir.y));
        return tileObj != null;
    }
    #endregion

    private void InitTile(int i, int j, TileType tileType, UseType useType, int size, bool startAnim = true, string layerName = "Default")
    {
        var tileInitData = _dataSetLoader.GetTileDataSet(tileType);
        if (tileInitData == null)
        {
            Debug.LogError($"タイルの初期データがありません -> index{i},{j}");
            return;
        }
        TileObj tileObj = _tilePool.GetTile();
        if (tileObj == null)
        {
            Debug.LogError($"タイルオブジェクトが取得できません -> index{i},{j},{tileType}");
            return;
        }
        Vector3 position = new Vector3(i * _tileSize, _tilePosY, j * _tileSize);
        int id = CalcID(i, j);
        tileObj.transform.position = position;
        tileObj.Init(tileInitData, _dataSetLoader.GetFrameTexture(), tileType, useType, id, startAnim, layerName);
        if (useType == UseType.Preview) return;

        AddActiveTile(id, DirectionType.None, tileObj);
    }

    private int CalcID(int x, int z)
    {
        return x * _gridSizeX + z;
    }

    public int PosToID(Vector3 pos)
    {
        float maxX = _gridSizeY * _tileSize;
        float maxZ = _gridSizeX * _tileSize;

        if (pos.x < 0 || pos.x >= maxX || pos.z < 0 || pos.z >= maxZ)
            return -1;

        var posIndex = GetPosIndex(pos);

        return CalcID(posIndex.x, posIndex.y);
    }

    private Vector2Int GetPosIndex(Vector3 pos)
    {
        int x = Mathf.RoundToInt(pos.x / _tileSize);
        int z = Mathf.RoundToInt(pos.z / _tileSize);

        return new Vector2Int(x, z);
    }

    public bool GetCornerPos(Vector3 pos, Vector2Int moveDir, out Vector3 target)
    {
        target = Vector3.zero;
        Vector3 checkPos = pos;
        var posIndex = GetPosIndex(pos);
        int startX = posIndex.x;
        int startY = posIndex.y;
        int length;
        if (Math.Abs(moveDir.x) == 1)
        {
            length = moveDir.x > 0 ? _stageData.Count - startX : startX;
            Debug.Log($"lengthx:{length}");
        }
        else
        {
            length = moveDir.y > 0 ? _stageData[0].Count - startY : startY;
            Debug.Log($"lengthy:{length}");
        }
        for (int i = 1; i <= length + 1; i++)
        {
            checkPos = checkPos + new Vector3(moveDir.x * _tileSize, 0, moveDir.y * _tileSize);
            TileObj tileObj = GetTileObj(checkPos);
            if (GetNextTileObj(checkPos, moveDir, out var nextTileObj) == false)
            {
                if (tileObj == null) return false;
                target = tileObj.transform.position;
                target.y = 0;
                return true;
            }
        }
        return false;
    }
}

