using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using TMPro;
using UnityEngine;

public class DialogView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _dialogText;
    [SerializeField] private RectTransform _dialogRoot;

    public void Initialized()
    {
        _dialogRoot.localScale = Vector3.zero;
        _dialogText.text = string.Empty;
    }

    public async UniTask ShowDialog(Vector3 target, float duration = 0.25f, CancellationToken token = default)
    {
        await _dialogRoot.DOScale(target, duration).ToUniTask(cancellationToken: token);
    }

    public void SetText(string message)
    {
        _dialogText.text = message;
    }
}
