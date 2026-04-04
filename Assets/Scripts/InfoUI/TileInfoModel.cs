using R3;
using System;
using System.Collections.Generic;
using System.Linq;

public class TileInfoModel
{
    public Observable<TileInfo> OnAddInfo => _onAddInfo;
    private Subject<TileInfo> _onAddInfo = new Subject<TileInfo>();
    private TileInfoDatas _tileInfoDatas;
    private readonly Dictionary<TileType, TileInfoKey> _tileInfoDict = new Dictionary<TileType, TileInfoKey>();
    private List<TileType> _verifiedTypes = new List<TileType>();

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

    public TileInfoModel(TileInfoDatas tileInfoDatas, List<TileType> tileTypes)
    {
        _tileInfoDatas = tileInfoDatas;
        InitDict();

        if (tileTypes == null || tileTypes.Count == 0)
        {
            _verifiedTypes = new List<TileType>();
            return;
        }
        _verifiedTypes = new List<TileType>(tileTypes);
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

        bool isVerified = true;

        foreach (var tileType in tileList)
        {
            if (!_verifiedTypes.Contains(tileType))
            {
                isVerified = false;
                _verifiedTypes.Add(tileType);
            }
        }

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

        var tileInfo = new TileInfo(
            isVerified,
            tileInfoKeys
            );

        _onAddInfo.OnNext(tileInfo);
    }

    private bool GetTileInfoKey(TileType tileType, out TileInfoKey tileInfoKey)
    {
        if (_tileInfoDict.TryGetValue(tileType, out tileInfoKey) == false)
        {
            return false;
        }
        return true;
    }

    public List<TileType> GetVerifiedTypes()
    {
        return new List<TileType>(_verifiedTypes);
    }

    public void ClearVerifiedTypes()
    {
        _verifiedTypes.Clear();
    }
}

public record TileInfo
{
    public readonly bool IsVerified;
    public List<TileInfoKey> TileInfoKeys;

    public TileInfo(bool isVerified, List<TileInfoKey> tileInfoKeys)
    {
        IsVerified = isVerified;
        TileInfoKeys = tileInfoKeys;
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
