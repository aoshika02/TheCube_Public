using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ResultView : MonoBehaviour
{
    [SerializeField] private RectTransform _bg;
    [SerializeField] private List<ResultLetter> _resultLetters;
    private const float TARGET_SCALE_Y = 2.14f;
    public void Initialize()
    {
        foreach (var letter in _resultLetters)
        {
            letter.Initialize();
        }
        _bg.gameObject.SetActive(false);
        _bg.localScale = new Vector3(0, 1, 1);
    }

    public async UniTask ShowFlow(float delay,float scaleDelay, float duration, float showTime, CancellationToken token)
    {
        _bg.gameObject.SetActive(true);
        await _bg.DOScaleX(1, duration).SetEase(Ease.Linear).ToUniTask(cancellationToken: token);
        foreach (var letter in _resultLetters)
        {
            letter.ShowCover();
            await UniTask.WaitForSeconds(delay, cancellationToken: token);
        }
        await UniTask.WhenAll(_resultLetters.Select(letter => letter.ShowFlow(TARGET_SCALE_Y, scaleDelay, duration, token)));
        await UniTask.WaitForSeconds(showTime, cancellationToken: token);
    }

    public void Shutdown()
    {
        foreach (var letter in _resultLetters)
        {
            letter.Shutdown();
        }
        _bg.gameObject.SetActive(false);
        _bg.localScale = new Vector3(0, 1, 1);
    }
}
