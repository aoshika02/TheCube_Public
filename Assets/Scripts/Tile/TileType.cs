using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

[JsonConverter(typeof(StringEnumConverter))]
public enum TileType 
{
    None        = -1,
    Normal      = 0,
    Goal        = 1,
    Skip        = 2,
    Reset       = 3,
    Warp        = 4,
    UpperArrow  = 5,
    LeftArrow   = 6,
    DownArrow   = 7,
    RightArrow  = 8,
    JumpUp      = 9,
    JumpDown    = 10,
    JumpLeft    = 11,
    JumpRight   = 12,
}