using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileInitDatas", menuName = "ScriptableObjects/TileInitDatas", order = 1)]
public class TileInitDatas : ScriptableObject
{
    public Texture2D FrameTexture;
    public List<TileInitData> TileInitDataList = new List<TileInitData>();
}

[System.Serializable]
public class TileInitData
{
    public TileType TileType;
    public Texture2D IconTexture;
    public Color BaseColor;
    public Color FrameColor;
    public Color IconColor;
}
