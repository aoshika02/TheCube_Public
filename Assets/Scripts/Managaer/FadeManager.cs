using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : SingletonMonoBehaviour<FadeManager>
{
    [SerializeField] private Image _fadeImage;
    private Material _fadeMaterial;
    private int _progressID = Shader.PropertyToID(ParamConsts.PROGRESS);
    private int _startPosID = Shader.PropertyToID(ParamConsts.START_POS);
    private const float _minProgress = 0f;
    private const float _maxProgress = 1f;
    private const float _leftStartPos = 0.25f;
    private const float _rightStartPos = 0.75f;
    protected override void Awake()
    {
        if (!CheckInstance())
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        _fadeMaterial = _fadeImage.material;
    }

    public async UniTask FadeIn(float duration = 0.5f)
    {
        _fadeImage.raycastTarget = true;
        await FadeAsync(true, duration);
    }

    public async UniTask FadeOut(float duration = 0.5f)
    {
        await FadeAsync(false, duration);
        _fadeImage.raycastTarget = false;
    }

    private async UniTask FadeAsync(bool isFadeIn, float duration = 0.5f)
    {
        float start = isFadeIn ? _minProgress : _maxProgress;
        float end = isFadeIn ? _maxProgress : _minProgress;
        float startPos = isFadeIn ? _leftStartPos : _rightStartPos;

        _fadeMaterial.SetFloat(_startPosID, startPos);
        await DOVirtual.Float(start, end, duration, f =>
        {
            _fadeMaterial.SetFloat(_progressID, f);
        });
    }
}
