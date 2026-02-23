using System;
using System.Collections.Generic;
using UnityEngine;

public class StageSaveManager : SingletonMonoBehaviour<StageSaveManager>
{
    private readonly Dictionary<int, StageSaveData> _stageSaveDataDic = new Dictionary<int, StageSaveData>();

    protected override void Awake()
    {
        if (!CheckInstance())
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void Init(int startStageID,int endStageID) 
    {
        for (int i = startStageID; i <= endStageID; i++)
        {
            SetSaveData(i, false);
        }
    }

    public void Init(List<StageSaveData> stageSaveDatas)
    {
        _stageSaveDataDic.Clear();
        foreach (var stageSaveData in stageSaveDatas)
        {
            SetSaveData(stageSaveData.StageID, stageSaveData.IsCleared);
        }
    }

    public void SetSaveData(int stageID, bool isClear)
    {
        if (_stageSaveDataDic.TryGetValue(stageID, out var stageSaveData))
        {
            if (isClear)
            {
                if (stageSaveData.IsCleared == true)
                {
                    _stageSaveDataDic[stageID] = 
                        new StageSaveData(
                            stageSaveData.StageID,
                            true,
                            stageSaveData.ClearCount + 1);
                }
                else
                {
                    //初回クリアであるため1をセットする
                    _stageSaveDataDic[stageID] = new StageSaveData(
                        stageSaveData.StageID,
                        true,
                        1);
                }
            }
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