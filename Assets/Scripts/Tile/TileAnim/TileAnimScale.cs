using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using TileAnimUtil;
using UnityEngine;

namespace TileAnimUtil
{
    public static partial class TileMaterialAnimUtil
    {
        public static async UniTask ScaleFlow(this Material material, CancellationTokenSource tokenSource, float duration = 3f)
        {
            if (!material.MaterialCheck()) return;
            Sequence seq = DOTween.Sequence()
                .Append(DOVirtual.Float(0f, 1f, 0, f =>
                {
                    material.SetFloat(_iconAlphaId, f);
                }))
                .Append(DOVirtual.Float(0.8f, 2.8f, duration, f =>
                {
                    material.SetFloat(_scaleId, f);
                    material.SetFloat(_iconAlphaId, 2.8f - f);
                }))
                .AppendInterval(duration / 5);
            await seq.SetLoops(-1).ToUniTask(cancellationToken: tokenSource.Token);
        }

        public static async UniTask ReverseScaleFlow(this Material material, CancellationTokenSource tokenSource, float duration = 3f)
        {
            if (!material.MaterialCheck()) return;
            Sequence seq = DOTween.Sequence()
                .Append(DOVirtual.Float(2.8f, 0.8f, duration, f =>
                {
                    material.SetFloat(_scaleId, f);
                    material.SetFloat(_iconAlphaId, f - 0.8f);
                }))
                .Append(DOVirtual.Float(1f, 0f, 0, f =>
                {
                    material.SetFloat(_iconAlphaId, f);
                }))
                .AppendInterval(duration / 5);
            await seq.SetLoops(-1).ToUniTask(cancellationToken: tokenSource.Token);
        }
    }
}

public class ScaleAnim : ITileAnim
{
    public async UniTask AnimFlow(Material material, CancellationTokenSource cts, float duration = 3f)
    {
        await material.ScaleFlow(cts, duration);
    }
}

public class ReverseScaleAnim : ITileAnim
{
    public async UniTask AnimFlow(Material material, CancellationTokenSource cts, float duration = 3f)
    {
        await material.ReverseScaleFlow(cts, duration);
    }
}
