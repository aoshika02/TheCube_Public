using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageDatas", menuName = "ScriptableObjects/StageDatas", order = 1)]
public class StageDatas : ScriptableObject
{
    public List<StageData> StageDataList;
}

[Serializable]
public class StageData
{
    public int StageID;
    public int StartID;
    public TextAsset StageJson;
}
