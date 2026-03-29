using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public interface ITileAnim
{
    UniTask AnimFlow(Material material, CancellationTokenSource cts, float duration = 3f);
}
