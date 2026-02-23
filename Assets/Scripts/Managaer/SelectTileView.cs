using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectTileView : SingletonMonoBehaviour<SelectTileView>
{
    [SerializeField] private int _stageSize = 1;
    [SerializeField] private int _minDistance = 8;

    private int _minStageID = 0;
    private int _maxStageID = 0;
    private int _viewLength = 5;
    private float _posY = -0.5f;
    private float _posZ = 0;

    private List<TileObj> _tileObjList = new List<TileObj>();

    private TilePool _tilePool;
    private DataSetLoader _dataSetLoader;

    public void SetStages(StageSelectData stageSelectData,int dir)
    {
        if (_tilePool == null)
        {
            _tilePool = TilePool.Instance;
        }
        if (_dataSetLoader == null)
        {
            _dataSetLoader = DataSetLoader.Instance;
        }

        _minStageID = stageSelectData.MinStageID;
        _maxStageID = stageSelectData.MaxStageID;
        _viewLength = stageSelectData.ViewLength;

        Release(stageSelectData.CurrentStageID, dir);
        for (int i = _minStageID; i <= _minStageID + _maxStageID * _viewLength; i++)
        {
            var obj = _tilePool.GetTile();
            var tileInitData = _dataSetLoader.GetTileDataSet(TileType.Normal);
            obj.Init(tileInitData, _dataSetLoader.GetFrameTexture(), TileType.None, UseType.Select);
            obj.transform.position = new Vector3(i * _stageSize, _posY, _posZ);
            _tileObjList.Add(obj);
        }
    }

    private void Release(int currentPosX, int dir)
    {
        if (_tileObjList.Count == 0) return;
        var tempList = new List<TileObj>(_tileObjList);
        foreach (var tileObj in tempList)
        {
            if (BeyondRange(tileObj, currentPosX, dir))
            {
                _tilePool.ReleaseTile(tileObj);
                _tileObjList.Remove(tileObj);
            }
        }
    }

    private bool BeyondRange(TileObj tileObj, int currentPosX, int dir)
    {
        if (tileObj.transform.position.x < _minStageID * _stageSize) return true;
        if (tileObj.transform.position.x > _maxStageID * _stageSize) return true;
        if (dir > 0)
            return currentPosX - _minDistance > tileObj.transform.position.x;
        else
            return currentPosX + _minDistance < tileObj.transform.position.x;
    }
}

public record StageSelectData
{
    public int CurrentStageID;
    public int MinStageID;
    public int MaxStageID;
    public int ViewLength;

    public StageSelectData(int CurrentStageID,int MinStageID,int MaxStageID,int ViewLength)
    {
        this.CurrentStageID = CurrentStageID;
        this.MinStageID = MinStageID;
        this.MaxStageID = MaxStageID;
        this.ViewLength = ViewLength;
    }
}
