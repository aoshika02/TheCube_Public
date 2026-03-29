using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectTileView : MonoBehaviour
{
    private const int VIEW_COUNT = 17;
    private const int HALF_VIEW = (VIEW_COUNT - 1) / 2;
    private const float POS_Y = -0.5f;
    private const float POS_Z = 0;

    private int _minPosX;
    private int _maxPosX;
    private TileDataSet _tileData;
    private Texture2D _frameTexture;
    private TilePool _tilePool;

    private int _edgeDir = 0;
    private int _edgeScrollCount = 0;

    private List<SelectTileInfo> _tileObjs = new List<SelectTileInfo>();

    public void Initialized(TileDataSet tileData, Texture2D frameTexture, TilePool tilePool, int minId, int maxID, int moveLength, int currentID)
    {
        _tileData = tileData;
        _frameTexture = frameTexture;
        _tilePool = tilePool;
        _minPosX = minId;
        _maxPosX = maxID * moveLength;
        _edgeDir = 0;
        _edgeScrollCount = currentID >= _maxPosX ? 1 : currentID <= _minPosX ? -1 : 0;
    }

    public void Scroll(int dir)
    {
        if (_tileObjs.Count == 0) return;
        if (dir == 0) return;

        int maxID = _tileObjs.Select(info => info.ID).Max();
        int minID = _tileObjs.Select(info => info.ID).Min();

        if (_edgeDir != 0)
        {
            if (dir == _edgeDir)
            {
                return;
            }
            _edgeScrollCount++;
            if (_edgeScrollCount >= HALF_VIEW)
            {
                _edgeDir = 0;
                _edgeScrollCount = 0;
            }
            return;
        }

        if (dir > 0)
        {
            if (maxID >= _maxPosX)
            {
                _edgeDir = 1;
                _edgeScrollCount = 0;
                return;
            }
            var selectTileInfo = _tileObjs.First(info => info.ID == minID);
            selectTileInfo.TileObj.transform.position += Vector3.right * VIEW_COUNT;
            selectTileInfo.ID += VIEW_COUNT;
        }
        else
        {
            if (minID <= _minPosX)
            {
                _edgeDir = -1;
                _edgeScrollCount = 0;
                return;
            }
            var selectTileInfo = _tileObjs.First(info => info.ID == maxID);
            selectTileInfo.TileObj.transform.position += Vector3.left * VIEW_COUNT;
            selectTileInfo.ID -= VIEW_COUNT;
        }
    }

    public void SpawnTile(int posX)
    {
        int startPosX = 0;
        if (MinLimitCheck(posX))
        {
            startPosX = _minPosX;
        }
        else if (MaxLimitCheck(posX))
        {
            startPosX = _maxPosX - VIEW_COUNT + 1;
        }
        else
        {
            startPosX = posX - HALF_VIEW;
        }

        for (int i = 0; i < VIEW_COUNT; i++)
        {
            var obj = _tilePool.GetTile();
            obj.Init(_tileData, _frameTexture, TileType.None, UseType.Select);
            obj.transform.position = new Vector3(startPosX, POS_Y, POS_Z);
            _tileObjs.Add(new SelectTileInfo(startPosX, obj));
            startPosX++;
        }
        _edgeDir = (posX == _minPosX) ? -1 : (posX == _maxPosX) ? 1 : 0;
        _edgeScrollCount = 0;
    }

    private bool MaxLimitCheck(int posX)
    {
        return posX > _maxPosX - HALF_VIEW;
    }

    private bool MinLimitCheck(int posX)
    {
        return posX < _minPosX + HALF_VIEW;
    }

    public void ShutDown()
    {
        _tileObjs.Clear();
    }
}

public class SelectTileInfo
{
    public int ID;
    public TileObj TileObj;

    public SelectTileInfo(int id, TileObj tileObj)
    {
        ID = id;
        TileObj = tileObj;
    }
}
