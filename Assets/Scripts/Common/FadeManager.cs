using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;
    private Material _fadeMaterial;
    private int _progressID = Shader.PropertyToID(ParamConsts.PROGRESS);
    private int _startPosID = Shader.PropertyToID(ParamConsts.START_POS);
    private const float MIN_PROGRESS = 0f;
    private const float MAX_PROGRESS = 1f;
    private const float LEFT_START_POS = 0.25f;
    private const float RIGHT_START_POS = 0.75f;

    public async UniTask FadeIn(float duration = 0.5f, CancellationToken token = default)
    {
        _fadeImage.raycastTarget = true;
        await FadeAsync(true, duration, token);
    }

    public async UniTask FadeOut(float duration = 0.5f, CancellationToken token = default)
    {
        await FadeAsync(false, duration, token);
        _fadeImage.raycastTarget = false;
    }

    private async UniTask FadeAsync(bool isFadeIn, float duration = 0.5f, CancellationToken token = default)
    {
        float start = isFadeIn ? MIN_PROGRESS : MAX_PROGRESS;
        float end = isFadeIn ? MAX_PROGRESS : MIN_PROGRESS;
        float startPos = isFadeIn ? LEFT_START_POS : RIGHT_START_POS;

        if(!TryGetFadeMaterial(out _fadeMaterial))
        {
            Debug.LogError("フェード用のマテリアルがありません");
            return;
        }
        _fadeMaterial.SetFloat(_startPosID, startPos);
        await DOVirtual.Float(start, end, duration, f =>
        {
            _fadeMaterial.SetFloat(_progressID, f);
        }).ToUniTask(cancellationToken: token);
    }

    private bool TryGetFadeMaterial(out Material fadeMaterial)
    {
        if (_fadeImage.material != null)
        {
            fadeMaterial = _fadeImage.material;
            return true;
        }
        fadeMaterial = null;
        return false;
    }
}
