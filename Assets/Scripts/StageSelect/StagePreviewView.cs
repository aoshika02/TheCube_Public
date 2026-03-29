using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.ObjectModel;
using System.Threading;
using TMPro;
using UnityEngine;

public class StagePreviewView : MonoBehaviour
{
    [SerializeField] private GameObject _stagePreviewCanvas;
    [SerializeField] private RectTransform _bounceRoot;
    [SerializeField] private RectTransform _stagePreviewRoot;
    [SerializeField] private TextMeshProUGUI _stageTitleText;
    [SerializeField] private TextMeshProUGUI _stageClearText;
    [SerializeField] private Transform _previewCamera;
    [SerializeField] private float _stageSize = 1f;

    public async UniTask ShowStagePreview(bool isShow, float duration = 0.3f, CancellationToken token = default)
    {
        await _stagePreviewRoot
            .DOScale(isShow ? Vector3.one : Vector3.zero, 0.3f)
            .SetEase(Ease.OutBack)
            .ToUniTask(cancellationToken: token);
    }

    public void SetCameraPos(ReadOnlyCollection<ReadOnlyCollection<TileType>> tileTypes)
    {
        int rows = tileTypes.Count;
        int cols = tileTypes.FindMaxCount();

        //中心に配置するための座標を計算
        float posX = (rows - 1) / 2f * _stageSize;
        float posY = rows > cols ? rows : cols;
        float posZ = (cols - 1) / 2f * _stageSize;

        _previewCamera.position = new Vector3(posX, posY, posZ);
    }

    public void SetCanvasPos(float posX)
    {
        _stagePreviewCanvas.transform.localPosition = new Vector3(
            posX,
            _stagePreviewCanvas.transform.localPosition.y,
            _stagePreviewCanvas.transform.localPosition.z);
    }

    public async UniTask BounceAnim(bool isShow, float delay = 0.25f, float duration = 0.5f, CancellationToken token = default)
    {
        await UniTask.WaitForSeconds(delay);
        await _bounceRoot
            .DOScale(isShow ? Vector3.one : Vector3.zero, duration)
            .SetEase(isShow ? Ease.OutBounce : Ease.InBounce)
            .ToUniTask(cancellationToken: token);
    }

    public void SetBounceScale(Vector3 scale)
    {
        _bounceRoot.localScale = scale;
    }

    public Vector3 GetCanvasPos()
    {
        return _stagePreviewCanvas.transform.localPosition;
    }

    public void SetStageTitle(int id)
    {
        _stageTitleText.text = $"{id:000}";
    }

    public void SetClear(bool isCleared)
    {
        if (isCleared)
        {
            _stageClearText.maxVisibleCharacters = _stageClearText.text.Length;
        }
        else
        {
            _stageClearText.maxVisibleCharacters = 0;
        }
    }
}
