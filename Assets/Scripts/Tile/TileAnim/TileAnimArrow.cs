using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using TileAnimUtil;
using UnityEngine;

namespace TileAnimUtil
{
    public static partial class TileMaterialAnimUtil
    {
        private static float GetStepRotate(TileType type)
        {
            return type switch
            {
                TileType.UpperArrow => -0.5f,
                TileType.LeftArrow  => 0f,
                TileType.DownArrow  => 0.5f,
                TileType.RightArrow => -1f,
                _ => 0f
            };
        }

        public static async UniTask ArrowFlow(this Material material, TileType type, CancellationTokenSource tokenSource, float duration = 3f)
        {
            if (!material.MaterialCheck()) return;
            material.SetFloat(_iconAlphaId, 1f);
            material.SetFloat(_scaleId, 1f);
            material.SetFloat(_rotateId, 0f);
            material.SetFloat(_stepRotateId, GetStepRotate(type));
            Sequence seq = DOTween.Sequence()
                .Append(DOVirtual.Float(0f, 0f, 0, f =>
                {
                    material.SetFloat(_borderId, f);
                }))
                .AppendInterval(0.25f)
                .Append(DOVirtual.Float(0f, 0.5f, 0, f =>
                {
                    material.SetFloat(_borderId, f);
                }))
                .AppendInterval(duration / 3)
                .Append(DOVirtual.Float(0.5f, 1f, 0, f =>
                {
                    material.SetFloat(_borderId, f);
                }))
                .AppendInterval(duration / 3);
            await seq.SetLoops(-1).ToUniTask(cancellationToken: tokenSource.Token);
        }
    }
}

public class ArrowAnim : ITileAnim
{
    private readonly TileType _tileType;

    public ArrowAnim(TileType tileType)
    {
        _tileType = tileType;
    }

    public async UniTask AnimFlow(Material material, CancellationTokenSource cts, float duration = 3f)
    {
        await material.ArrowFlow(_tileType, cts, duration);
    }
}
