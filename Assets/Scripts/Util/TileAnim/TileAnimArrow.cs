using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;

namespace TileAnimUtil
{
    public static partial class TileMaterialAnimUtil 
    {
        private static float GetStepRotate(ArrowType type)
        {
            return type switch
            {
                ArrowType.Up => -0.5f,
                ArrowType.Left => 0f,
                ArrowType.Down => 0.5f,
                ArrowType.Right => -1f,
                _ => 0f
            };
        }

        public static async UniTask ArrowFlow(this Material material, ArrowType type, CancellationTokenSource tokenSource, float duration = 3f)
        {
            if (!material.MaterialCheck()) return;
            material.SetFloat(_iconAlphaId, 1f);
            material.SetFloat(_scaleId, 1f);
            material.SetFloat(_rotateId, 0f);
            material.SetFloat(_stepRotateId, GetStepRotate(type));
            Sequence seq = DOTween.Sequence();
            seq.Append(DOVirtual.Float(0f, 0f, 0, f =>
            {
                material.SetFloat(_borderId, f);
            }));
            seq.AppendInterval(0.25f);
            seq.Append(DOVirtual.Float(0f, 0.5f, 0, f =>
            {
                material.SetFloat(_borderId, f);
            }));
            seq.AppendInterval(duration / 3);
            seq.Append(DOVirtual.Float(0.5f, 1f, 0, f =>
            {
                material.SetFloat(_borderId, f);
            }));
            seq.AppendInterval(duration / 3);
            await seq.SetLoops(-1).ToUniTask(cancellationToken: tokenSource.Token);
        }
    }

    public enum ArrowType
    {
        Zero,
        Up,
        Down,
        Left,
        Right
    }
}
