using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;

namespace TileAnimUtil
{
    public static partial class TileMaterialAnimUtil
    {

        public static async UniTask RotateFlow(this Material material, CancellationTokenSource tokenSource, float duration = 3f)
        {
            if (!material.MaterialCheck()) return;
            material.SetFloat(_iconAlphaId, 1f);
            material.SetFloat(_scaleId, 1f);
            material.SetFloat(_rotateId, 1f);

            await DOVirtual.Float(1f, 0f, duration, f =>
            {
                material.SetFloat(_rotateId, f);
            }).SetLoops(-1).ToUniTask(cancellationToken: tokenSource.Token);
        }
    }
}
