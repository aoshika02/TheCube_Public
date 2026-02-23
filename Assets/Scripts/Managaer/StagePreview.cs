using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;

public class StagePreview : SingletonMonoBehaviour<StagePreview>
{
    [SerializeField] private GameObject _stagePreviewCanvas;
    [SerializeField] private RectTransform _bounceRoot;
    [SerializeField] private RectTransform _stagePreviewRoot;
    [SerializeField] private TextMeshProUGUI _stageTitleText;
    [SerializeField] private TextMeshProUGUI _stageClearText;

    [SerializeField] private Transform _previewCamera;
    [SerializeField] private float _stageSize = 1f;

    private DataSetLoader _dataLoader;
    private int _currentStageID = -1;
    public void Init()
    {
        _currentStageID = -1;
        _dataLoader = DataSetLoader.Instance;
    }

    public void UpdatePreviewStage(StageSaveData stageSaveData)
    {
        if (_currentStageID == stageSaveData.StageID) return;

        TilePool.Instance.ReleaseAll(UseType.Preview);
        var stageData = _dataLoader.GetStageDataSet(stageSaveData.StageID);
        if (stageData != null)
        {
            var data = stageData.TileTypeData;

            SetCameraPos(data);

            TileManager.Instance.Init(data, UseType.Preview, false, "Preview");
            Debug.Log($"ステージプレビュー: {stageSaveData.StageID} を表示");
        }
        _currentStageID = stageSaveData.StageID;
        //IDは0始まりのため +1して3桁表示する
        _stageTitleText.text = $"{stageSaveData.StageID + 1:000}";

        if (stageSaveData.IsCleared)
        {
            _stageClearText.maxVisibleCharacters = _stageClearText.text.Length;
        }
        else
        {
            _stageClearText.maxVisibleCharacters = 0;
        }
    }

    public async UniTask ShowStagePreview(bool isShow, float duration = 0.3f)
    {
        await _stagePreviewRoot
            .DOScale(isShow ? Vector3.one : Vector3.zero, 0.3f)
            .SetEase(Ease.OutBack)
            .ToUniTask();
    }

    private void SetCameraPos(ReadOnlyCollection<ReadOnlyCollection<TileType>> tileTypes)
    {
        int rows = tileTypes.Count;
        int cols = tileTypes.FindMaxCount();

        //中心に配置するための座標を計算
        float posX = (rows - 1) / 2f * _stageSize;
        float posY = rows > cols ? rows : cols;
        float posZ = (cols - 1) / 2f * _stageSize;

        Debug.Log($"rows: {rows}, cols: {cols}, pos: ({posX},{posY},{posZ})");
        _previewCamera.position = new Vector3(posX, posY, posZ);
    }

    public void SetCanvasPos(float posX)
    {
        _stagePreviewCanvas.transform.localPosition = new Vector3(
            posX,
            _stagePreviewCanvas.transform.localPosition.y,
            _stagePreviewCanvas.transform.localPosition.z);
    }

    public async UniTask BounceAnim(bool isShow, float delay = 0.25f, float duration = 0.5f)
    {
        await UniTask.WaitForSeconds(delay);
        await _bounceRoot
            .DOScale(isShow ? Vector3.one : Vector3.zero, duration)
            .SetEase(isShow ? Ease.OutBounce : Ease.InBounce)
            .ToUniTask();
    }

    public void SetBounceScale(Vector3 scale)
    {
        _bounceRoot.localScale = scale;
    }

    public Vector3 GetCanvasPos()
    {
        return _stagePreviewCanvas.transform.localPosition;
    }
}