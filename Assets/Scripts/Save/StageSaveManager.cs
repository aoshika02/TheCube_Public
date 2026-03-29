using System;
using System.Collections.Generic;
using UnityEngine;

public class StageSaveManager
{
    private readonly Dictionary<int, StageSaveData> _stageSaveDataDic = new Dictionary<int, StageSaveData>();

    public StageSaveManager(int startStageID,int endStageID)
    {
        _stageSaveDataDic.Clear();
        for (int i = startStageID; i <= endStageID; i++)
        {
            SetSaveData(i, false);
        }
    }

    public StageSaveManager(List<StageSaveData> stageSaveDatas,int endStageID)
    {
        _stageSaveDataDic.Clear();
        foreach (var stageSaveData in stageSaveDatas)
        {
            SetSaveData(stageSaveData.StageID, stageSaveData.IsCleared);
        }

        for (int i = 0; i <= endStageID; i++)
        {
            if (_stageSaveDataDic.ContainsKey(i)) continue;
            SetSaveData(i, false);
        }
    }

    public void SetSaveData(int stageID, bool isClear)
    {
        if (_stageSaveDataDic.TryGetValue(stageID, out var stageSaveData))
        {
            if (!isClear) return;

            int newCount = stageSaveData.IsCleared
                ? stageSaveData.ClearCount + 1
                : 1;
            _stageSaveDataDic[stageID] = new StageSaveData(stageID, true, newCount);
            return;
        }

        if (_stageSaveDataDic.TryAdd(stageID,
            new StageSaveData(
                stageID,
                isClear,
                isClear ? 1 : 0)
            ) == false)
        {
            Debug.LogError($"ステージセーブデータの追加に失敗しました。StageID:{stageID}");
        }
    }

    public bool GetSaveData(int stageID, out StageSaveData stageSaveData) 
    {
        if(_stageSaveDataDic.TryGetValue(stageID, out stageSaveData))
        {
            return true;
        }
        stageSaveData = null;
        return false;
    }

    public List<StageSaveData> GetAllSaveData()
    {
        return new List<StageSaveData>(_stageSaveDataDic.Values);
    }
}

[Serializable]
public record StageSaveData
{
    public readonly int StageID;
    public readonly int ClearCount;
    public readonly bool IsCleared;

    public StageSaveData(int stageID, bool isCleared, int clearCount)
    {
        StageID = stageID;
        IsCleared = isCleared;
        ClearCount = clearCount;
    }
}