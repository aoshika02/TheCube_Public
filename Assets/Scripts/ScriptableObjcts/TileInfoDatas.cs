using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileInfoDatas", menuName = "ScriptableObjects/TileInfoDatas", order = 1)]
public class TileInfoDatas : ScriptableObject    
{
    public List<TileInfoData> TileInfoDataList = new List<TileInfoData>();
}

[Serializable]
public class TileInfoData
{
    public TileType TileType;
    public DialogEventType DialogEventType;
}
