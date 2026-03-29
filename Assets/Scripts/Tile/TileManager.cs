using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using VContainer;

public partial class TileManager : MonoBehaviour
{
    [SerializeField] private int _gridSizeX;
    [SerializeField] private int _gridSizeY;
    [SerializeField] private float _tileSize = 1f;
    [SerializeField] private float _tilePosY = -0.575f;

    public float TileSize => _tileSize;

    private DataSetLoader _dataSetLoader;
    private TilePool _tilePool;
    private TileAnimLocator _tileAnimLocator;

    [Inject]
    public void Construct(DataSetLoader dataSetLoader, TilePool tilePool, TileAnimLocator tileAnimLocator)
    {
        _dataSetLoader = dataSetLoader;
        _tilePool = tilePool;
        _tileAnimLocator = tileAnimLocator;
    }

    public void Init(ReadOnlyCollection<ReadOnlyCollection<TileType>> tileTypes, UseType useType = UseType.Stage, bool startAnim = true, string layerName = "Default")
    {
        if (tileTypes == null)
        {
            Debug.LogError("タイルタイプのリストがありません");
            return;
        }

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
                InitTile(j, i, type, useType, startAnim, layerName);
            }
            row.Clear();
        }
    }

    #region タイル取得

    public TileObj GetTileObj(int id)
    {
        if (GetActiveTileData(id, out var tileObj))
        {
            if (tileObj == null) return null;
            return tileObj;
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

    #endregion

    private void InitTile(int i, int j, TileType tileType, UseType useType, bool startAnim = true, string layerName = "Default")
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
        tileObj.Init(tileInitData, _dataSetLoader.GetFrameTexture(), tileType, useType, _tileAnimLocator.Resolve(tileType), startAnim, layerName);
        if (useType == UseType.Preview) return;

        AddActiveTile(id, tileObj);
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
}
