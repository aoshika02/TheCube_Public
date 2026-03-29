using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VContainer;

public class LoadManager
{
    private GameSaveData _gameSaveData;
    private SavePath _path;

    public LoadManager(SavePath savePath)
    {
        _path = savePath;
    }

    public bool GetGameSaveData(out GameSaveData gameSaveData, bool isForce = false)
    {
        if (_gameSaveData == null || isForce == true)
        {
            if (SaveLoadUtil.Load(_path.GetSavePath(), out _gameSaveData))
            {

            }
        }
        gameSaveData = _gameSaveData;
        return gameSaveData != null;
    }

    public void ClearCache()
    {
        _gameSaveData = null;
    }

    public bool IsTutorialFinished()
    {
        if (GetGameSaveData(out var saveData))
        {
            return saveData != null && saveData.IsTutorialFinished;
        }
        return false;
    }
}

[Serializable]
public record GameSaveData
{
    public bool IsTutorialFinished;
    public int LastStageID;
    public ReadOnlyCollection<StageSaveData> StageSaveDatas;

    public GameSaveData() { }
    public GameSaveData(bool tutorialFinished, int lastStageID, List<StageSaveData> stageSaveDatas)
    {
        IsTutorialFinished = tutorialFinished;
        LastStageID = lastStageID;
        StageSaveDatas = stageSaveDatas.AsReadOnly();
    }
}