using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ResultLetter : MonoBehaviour
{
    [SerializeField] private Image _coverImage;
    [SerializeField] private GameObject _letterObject;
    
    public void Initialize()
    {
        _coverImage.gameObject.SetActive(false);
        _letterObject.SetActive(false);
    }

    public void ShowCover()
    {
        _coverImage.gameObject.SetActive(true);
    }

    public async UniTask ShowFlow(float scaleY, float delay, float duration, CancellationToken token)
    {
        await _coverImage.transform
            .DOScaleY(scaleY, duration)
            .SetEase(Ease.Linear)
            .ToUniTask(cancellationToken: token);
        await UniTask.WaitForSeconds(delay, cancellationToken: token);
        _letterObject.SetActive(true);
        _coverImage.gameObject.SetActive(false);
    }

    public void Shutdown()
    {
        _coverImage.gameObject.SetActive(false);
        _coverImage.transform.localScale = Vector3.one;
        _letterObject.SetActive(false);
    }
}
