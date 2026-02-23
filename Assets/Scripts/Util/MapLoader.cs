using Newtonsoft.Json;
using System.Collections.Generic;

public static class MapLoader
{
    public static List<List<TileType>> Load(string json)
    {
        return JsonConvert.DeserializeObject<MapData>(json)?.map;
    }
}
public record MapData
{
    public List<List<TileType>> map;
}

