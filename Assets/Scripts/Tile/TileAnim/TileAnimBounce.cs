using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using TileAnimUtil;
using UnityEngine;

namespace TileAnimUtil
{
    public static partial class TileMaterialAnimUtil
    {
        public static async UniTask BounceFlow(this Material material, CancellationTokenSource tokenSource, float duration = 0.5f)
        {
            if (!material.MaterialCheck()) return;
            Sequence seq = DOTween.Sequence()
                .Append(DOVirtual.Float(0.9f, 1.2f, duration, f =>
                {
                    material.SetFloat(_scaleId, f);
                }))
                .Append(DOVirtual.Float(1.2f, 0.9f, duration, f =>
                {
                    material.SetFloat(_scaleId, f);
                }));
            await seq.SetLoops(-1).ToUniTask(cancellationToken: tokenSource.Token);
        }
    }
}

public class BounceAnim : ITileAnim
{
    public async UniTask AnimFlow(Material material, CancellationTokenSource cts, float duration = 3f)
    {
        await material.BounceFlow(cts);
    }
}
