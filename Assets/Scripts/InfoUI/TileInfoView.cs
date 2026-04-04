using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class TileInfoView : MonoBehaviour
{
    [SerializeField] private Transform _infoViewParent;
    [SerializeField] private TileInfoDatas _tileInfoDatas;
    [SerializeField] private ContentSizeFitter _frameSizeFitter;
    private List<InfoView> _infoViews = new List<InfoView>();
    private InfoViewPool _infoViewPool;
    private DataSetLoader _dataSetLoader;
    private TextDataset _textDataset;

    public void Initialized(InfoViewPool infoViewPool, DataSetLoader dataSetLoder, TextDataset textDataset)
    {
        _infoViewPool = infoViewPool;
        _dataSetLoader = dataSetLoder;
        _textDataset = textDataset;
    }

    public async UniTask AddInfoViews(List<TileInfoKey> tileInfoKeys, CancellationToken token = default)
    {
        foreach (var infoKey in tileInfoKeys)
        {
            AddInfoView(infoKey.TileType, infoKey.DialogEventType);
        }
        await UniTask.Yield(cancellationToken: token);
        RebuildLayout();
    }

    public void AddInfoView(TileType tileType, DialogEventType dialogEventType)
    {
        var tileDataSet = _dataSetLoader.GetTileDataSet(tileType);
        if (tileDataSet == null) return;
        var infoView = _infoViewPool.GetInfoView(_infoViewParent);
        infoView.SetInfo(tileDataSet.IconTexture, tileDataSet.IconColor, _textDataset.GetText(dialogEventType));
        _infoViews.Add(infoView);
    }

    public async UniTask SetActiveInfo(bool? isActive = null, CancellationToken token = default)
    {
        if (_infoViews == null || _infoViews.Count == 0) return;
        _infoViews.ForEach(x =>
        {
            x.gameObject.SetActive(isActive.HasValue ? isActive.Value : !x.gameObject.activeSelf);
        });
        await UniTask.Yield(cancellationToken: token);
        RebuildLayout();
    }

    private void RebuildLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_frameSizeFitter.GetComponent<RectTransform>());
    }

    public void ClearInfoViews()
    {
        if (_infoViews == null || _infoViews.Count == 0) return;
        _infoViews.ForEach(x =>
        {
            _infoViewPool.ReleaseInfoView(x);
        });
        _infoViews.Clear();
    }

}
