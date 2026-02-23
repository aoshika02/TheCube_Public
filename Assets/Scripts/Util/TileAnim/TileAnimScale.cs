using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;


namespace TileAnimUtil
{
    public static partial class TileMaterialAnimUtil
    {
        public static async UniTask ScaleFlow(this Material material, CancellationTokenSource tokenSource, float duration = 3f)
        {
            if (!material.MaterialCheck()) return;
            Sequence seq = DOTween.Sequence();
            seq.Append(DOVirtual.Float(0f, 1f, 0, f =>
            {
                material.SetFloat(_iconAlphaId, f);
            }));
            seq.Append(DOVirtual.Float(0.8f, 2.8f, duration, f =>
            {
                material.SetFloat(_scaleId, f);
                material.SetFloat(_iconAlphaId, 2.8f - f);
            }));
            seq.AppendInterval(duration / 5);
            await seq.SetLoops(-1).ToUniTask(cancellationToken: tokenSource.Token);
        }

     public static async UniTask ReverseScaleFlow(this Material material, CancellationTokenSource tokenSource, float duration = 3f)
        {
            if (!material.MaterialCheck()) return;
            Sequence seq = DOTween.Sequence();
            seq.Append(DOVirtual.Float(2.8f, 0.8f, duration, f =>
            {
                material.SetFloat(_scaleId, f);
                material.SetFloat(_iconAlphaId, f - 0.8f);
            }));
            seq.Append(DOVirtual.Float(1f, 0f, 0, f =>
            {
                material.SetFloat(_iconAlphaId, f);
            }));
            seq.AppendInterval(duration / 5);
            await seq.SetLoops(-1).ToUniTask(cancellationToken: tokenSource.Token);
        }
    }
}
