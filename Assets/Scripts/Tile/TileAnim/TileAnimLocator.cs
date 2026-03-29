using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TileAnimLocator
{
    private readonly Dictionary<TileType, ITileAnim> _animDict = new();
    private readonly ITileAnim _defaultAnim;

    public TileAnimLocator(ITileAnim defaultAnim)
    {
        _defaultAnim = defaultAnim;
    }

    public void Register(TileType tileType, ITileAnim tileAnim)
    {
        _animDict[tileType] = tileAnim;
    }

    public ITileAnim Resolve(TileType tileType)
    {
        return _animDict.TryGetValue(tileType, out var anim) ? anim : _defaultAnim;
    }
}

public class EmptyTileAnim : ITileAnim
{
    public UniTask AnimFlow(Material material, CancellationTokenSource cts, float duration = 3f)
    {
        return UniTask.CompletedTask;
    }
}
