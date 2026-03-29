using System.Collections.Generic;

public class TileEventLocator
{
    private readonly Dictionary<TileType, ITileEvent> _eventDict = new Dictionary<TileType, ITileEvent>();
    private readonly ITileEvent _defaultEvent;

    public TileEventLocator(ITileEvent defaultEvent) 
    {
        _defaultEvent = defaultEvent;
    }

    public void Register(TileType tileType, ITileEvent tileEvent)
    {
        _eventDict[tileType] = tileEvent;
    }

    public ITileEvent Resolve(TileType tileType)
    {
        return _eventDict.TryGetValue(tileType, out var tileEvent) ? tileEvent : _defaultEvent;
    }
}
