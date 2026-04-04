using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VContainer;

public class SaveManager
{
    private StageSaveManager _stageSaveManager;
    private TutorialManager _tutorialManager;
    private LoadManager _loadManager;
    private SavePath _savePath;
    private TileInfoModel _tileInfoModel;

    [Inject]
    public void Construct(StageSaveManager stageSaveManager, LoadManager loadManager, SavePath savePath, TutorialManager tutorialManager,TileInfoModel tileInfoModel)
    {
        _stageSaveManager = stageSaveManager;
        _loadManager = loadManager;
        _savePath = savePath;
        _tutorialManager = tutorialManager;
        _tileInfoModel = tileInfoModel;
    }
    public bool Save(int stageIndex)
    {
        if (SaveLoadUtil.DirectoryCheck(_savePath.GetDirectoryPath()) == false) return false;
        if (MakeSaveData(stageIndex, out var saveData) == false) return false;

        var savePath = _savePath.GetSavePath();
        string saveTempPath = savePath + ".tmp";
        string backupPath = savePath + ".bak";
        try
        {
            bool saveSuccess = SaveLoadUtil.MakeJson(saveData, saveTempPath);

            if (saveSuccess)
            {
                if (File.Exists(savePath))
                {
                    File.Replace(saveTempPath, savePath, backupPath);
                    SaveLoadUtil.TryDelete(backupPath);
                }
                else
                {
                    File.Move(saveTempPath, savePath);
                }
            }
            else
            {
                Debug.LogWarning($"{typeof(GameSaveData)} のセーブに失敗しました");
            }
            return saveSuccess;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"セーブ中に例外発生: {e.Message}");
            if (File.Exists(backupPath) && !File.Exists(savePath))
            {
                File.Move(backupPath, savePath);
            }
            return false;
        }
        finally
        {
            SaveLoadUtil.TryDelete(saveTempPath);
            SaveLoadUtil.TryDelete(backupPath);
        }

    }

    public bool DeleteSaveData()
    {
        var savePath = _savePath.GetSavePath();
        try
        {
            if (SaveLoadUtil.TryDelete(savePath))
            {
                _loadManager.ClearCache();
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"セーブデータ削除失敗: {savePath} ({e.Message})");
            return false;
        }
    }

    private bool MakeSaveData(int stageIndex, out GameSaveData outSaveData)
    {
        try
        {
            outSaveData = new GameSaveData(
                _tutorialManager.IsFinished,
                stageIndex,
                _stageSaveManager.GetAllSaveData(),
                _tileInfoModel.GetVerifiedTypes()
                );
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            outSaveData = null;
            return false;
        }
        return true;
    }
}

public class SavePath
{
    public const string DirectoryName = "_save";
    public const string FileName = "stage_save_data";
    private List<string> _pathes = new List<string>();
    public SavePath()
    {
        _pathes = new List<string>();
        _pathes.Add(SaveLoadUtil.GetBasePath());
        _pathes.Add(DirectoryName);
    }

    public string GetDirectoryPath()
    {
        return SaveLoadUtil.MakePath(_pathes);
    }

    public string GetSavePath()
    {
        List<string> pathes = new List<string>(_pathes);
        pathes.Add(FileName);
        return SaveLoadUtil.MakePath(pathes);
    }
}