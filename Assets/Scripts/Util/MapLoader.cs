using Newtonsoft.Json;
using System.Collections.Generic;

public static class MapLoader
{
    public static List<List<TileType>> Load(string json)
    {
        return JsonConvert.DeserializeObject<MapData>(json)?.Map;
    }
}
public record MapData
{
    [JsonProperty("map")]
    public List<List<TileType>> Map;
}

