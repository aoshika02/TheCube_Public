using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TileInfoSystem : SingletonMonoBehaviour<TileInfoSystem>
{
    [SerializeField] private Transform _infoViewParent;
    [SerializeField] private TileInfoDatas _tileInfoDatas;
    [SerializeField] private ContentSizeFitter _frameSizeFitter;
    private List<InfoView> _infoViews = new List<InfoView>();
    private readonly Dictionary<TileType, DialogEventType> _tileInfoDict = new Dictionary<TileType, DialogEventType>();
    private InfoViewPool _infoViewPool;
    private DataSetLoader _dataSetLoader;
    private TextDataset _textDataset;

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

    protected override void Awake()
    {
        if (CheckInstance() == false) return;
        _infoViewPool = InfoViewPool.Instance;
        _dataSetLoader = DataSetLoader.Instance;
        _textDataset = TextDataset.Instance;

        foreach (var tileInfoData in _tileInfoDatas.TileInfoDataList)
        {
            if (_tileInfoDict.TryAdd(tileInfoData.TileType, tileInfoData.DialogEventType) == false)
            {
                Debug.LogWarning($"TileType {_tileInfoDict} が重複しています");
            }
        }
    }
    public void SetInfos(StageDataSet stageDataSet)
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
        if(hasJump || hasJumpRight)
        {
            tileList = tileList.Where(x => !_removeJumpSet.Contains(x)).ToList();
            if (hasJumpRight == false)
            {
                tileList.Add(TileType.JumpRight);
            }
        }


        foreach (TileType tileType in Enum.GetValues(typeof(TileType)))
        {
            if (tileList.Contains(tileType))
            {
                AddInfoView(tileType);
            }
        }
    }

    public void RebuildLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_frameSizeFitter.GetComponent<RectTransform>());
    }

    public void SwitchInfoViewActive()
    {
        if (_infoViews == null || _infoViews.Count == 0) return;
        _infoViews.ForEach(x =>
        {
            x.gameObject.SetActive(!x.gameObject.activeSelf);
        });
    }

    private void AddInfoView(TileType tileType)
    {
        var tileDataSet = _dataSetLoader.GetTileDataSet(tileType);
        if (tileDataSet == null) return;
        var infoView = _infoViewPool.GetInfoView(_infoViewParent);
        infoView.SetInfo(tileDataSet.IconTexture, tileDataSet.IconColor, GetInfoText(tileType));
        _infoViews.Add(infoView);
    }

    private string GetInfoText(TileType tileType)
    {
        if (_tileInfoDict.TryGetValue(tileType, out var dialogEventType) == false)
        {
            return string.Empty;
        }
        return _textDataset.GetText(dialogEventType);
    }
}
