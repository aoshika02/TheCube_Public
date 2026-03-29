using R3;
using System;
using System.Collections.Generic;
using System.Linq;

public class TileInfoModel
{
    public Observable<List<TileInfoKey>> OnAddInfo => _onAddInfo;
    private Subject<List<TileInfoKey>> _onAddInfo = new Subject<List<TileInfoKey>>();
    private TileInfoDatas _tileInfoDatas;
    private readonly Dictionary<TileType, TileInfoKey> _tileInfoDict = new Dictionary<TileType, TileInfoKey>();

    TileType[] _removeSet = new TileType[]
    {
         TileType.None,
         TileType.Normal,
    };

    TileType[] _removeArrowSet = new TileType[]
    {
        TileType.UpperArrow,
        TileType.LeftArrow,
        TileType.DownArrow,
    };

    TileType[] _removeJumpSet = new TileType[]
    {
         TileType.JumpUp,
         TileType.JumpDown,
         TileType.JumpLeft,
    };

    public TileInfoModel(TileInfoDatas tileInfoDatas)
    {
        _tileInfoDatas = tileInfoDatas;
        InitDict();
    }

    private void InitDict()
    {
        foreach (var tileInfoData in _tileInfoDatas.TileInfoDataList)
        {
            if (_tileInfoDict.TryAdd(tileInfoData.TileType, new(tileInfoData)) == false)
            {
                UnityEngine.Debug.LogWarning($"TileType {_tileInfoDict} が重複しています");
            }
        }
    }

    public void Initialize(StageDataSet stageDataSet)
    {
        if (stageDataSet == null) return;
        List<TileType> tileList = stageDataSet.TileTypeData
            .SelectMany(x => x)
            .Distinct()
            .Where(x => !_removeSet.Contains(x))
            .ToList();

        bool hasArrow = tileList.Any(x => _removeArrowSet.Contains(x));
        bool hasRightArrow = tileList.Contains(TileType.RightArrow);
        if (hasArrow || hasRightArrow)
        {
            tileList = tileList.Where(x => !_removeArrowSet.Contains(x)).ToList();
            if (hasRightArrow == false)
            {
                tileList.Add(TileType.RightArrow);
            }
        }

        bool hasJump = tileList.Any(x => _removeJumpSet.Contains(x));
        bool hasJumpRight = tileList.Contains(TileType.JumpRight);
        if (hasJump || hasJumpRight)
        {
            tileList = tileList.Where(x => !_removeJumpSet.Contains(x)).ToList();
            if (hasJumpRight == false)
            {
                tileList.Add(TileType.JumpRight);
            }
        }

        List<TileInfoKey> tileInfoKeys = new List<TileInfoKey>();

        foreach (TileType tileType in Enum.GetValues(typeof(TileType)))
        {
            if (tileList.Contains(tileType) && GetTileInfoKey(tileType, out var tileInfoKey))
            {
                tileInfoKeys.Add(tileInfoKey);
            }
        }

        _onAddInfo.OnNext(tileInfoKeys);
    }

    private bool GetTileInfoKey(TileType tileType, out TileInfoKey tileInfoKey)
    {
        if (_tileInfoDict.TryGetValue(tileType, out tileInfoKey) == false)
        {
            return false;
        }
        return true;
    }
}

public record TileInfoKey
{
    public readonly TileType TileType;
    public readonly DialogEventType DialogEventType;
    public TileInfoKey(TileType tileType, DialogEventType dialogEventType)
    {
        TileType = tileType;
        DialogEventType = dialogEventType;
    }

    public TileInfoKey(TileInfoData tileInfoData)
    {
        TileType = tileInfoData.TileType;
        DialogEventType = tileInfoData.DialogEventType;
    }
}
