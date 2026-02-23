using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using UnityEngine;

public class SaveLoadManager : SingletonMonoBehaviour<SaveLoadManager>
{
    private List<string> _pathes = new List<string>();
    private readonly string _directoryName = "_save";
    private readonly string _fileName = "stage_save_data";
    public bool IsLoaded => _isLoaded;
    private bool _isLoaded = false;
    private GameSaveData _gameSaveData;
    public const int MIN_SAVE_INDEX = 1;
    protected override void Awake()
    {
        if (CheckInstance() == false) return;
        DontDestroyOnLoad(gameObject);
        _isLoaded = false;
        _pathes = new List<string>();
        _pathes.Add(GetBasePath());
        _pathes.Add(_directoryName);
        _isLoaded = true;
    }

    public void Save(int stageIndex)
    {
        if (DirectoryCheck(MakePath(_pathes)) == false) return;
        if (MakeSaveData(stageIndex, out var saveData) == false) return;

        List<string> pathes = new List<string>(_pathes);
        pathes.Add(_fileName);
        string saveTempPath = MakePath(pathes) + ".tmp";
        var finalPath = MakePath(pathes);

        try
        {
            bool saveSuccess = MakeJson(saveData, saveTempPath);

            if (saveSuccess)
            {
                if (TryDelete(finalPath))
                {
                    File.Move(saveTempPath, finalPath);
                    Debug.Log($"{typeof(GameSaveData)} のセーブに成功しました");
                }
            }
            else
            {
                Debug.LogWarning($"{typeof(GameSaveData)} のセーブに失敗しました");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"セーブ中に例外発生: {e.Message}");
        }
        finally
        {
            TryDelete(saveTempPath);
        }

    }

    private bool MakeSaveData(int stageIndex, out GameSaveData outSaveData)
    {
        // セーブに必要なコンポーネントが存在するか
        bool isExist =
            StageSaveManager.Instance != null &&
            TutorialManager.Instance != null;

        if (!isExist)
        {
            Debug.LogWarning("セーブに必要なコンポーネントが存在しません");
            outSaveData = null;
            return false;
        }

        try
        {
            outSaveData = new GameSaveData(
                TutorialManager.Instance.IsFinished,
                stageIndex,
                StageSaveManager.Instance.GetAllSaveData()
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

    private bool TryDelete(string path)
    {
        try
        {
            if (File.Exists(path)) File.Delete(path);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"ファイル削除失敗: {path} ({e.Message})");
            return false;
        }
    }

    public bool DeleteSaveData()
    {
        List<string> pathes = new List<string>(_pathes);
        pathes.Add(_fileName);
        var savePath = MakePath(pathes);
        try
        {
            if (TryDelete(savePath))
            {
                _gameSaveData = null;
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

    /// <summary>
    /// フォルダ生成
    /// </summary>
    /// <param name="folderPath"></param>
    private bool DirectoryCheck(string folderPath)
    {
        try
        {
            // フォルダがなければ作成
            Directory.CreateDirectory(folderPath);
            Debug.Log("フォルダ確認＆作成完了");
            return true;
        }
        catch
        {
            Debug.LogError("フォルダの確認＆作成に失敗");
            return false;
        }
    }
    private string MakePath(List<string> paths)
    {
        return string.Join("/", paths);
    }

    private string GetBasePath()
    {
#if UNITY_EDITOR
        return Application.dataPath;
#else
    return Application.persistentDataPath;
#endif
    }
    /// <summary>
    /// セーブデータの取得
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="index"></param>
    /// <returns></returns>
    private bool Load<T>(out T outData) where T : new()
    {
        try
        {
            List<string> pathes = new List<string>(_pathes);
            pathes.Add(_fileName);
            using (StreamReader rd = new StreamReader(MakePath(pathes)))
            {
                string json = rd.ReadToEnd();
                outData = JsonConvert.DeserializeObject<T>(json);
                return true;
            }
        }
        catch
        {
            Debug.LogWarning($"セーブデータの読み込みに失敗しました log : {_fileName} is not found");
            outData = default;
            return false;
        }
    }
    /// <summary>
    /// セーブデータのjson書き出し
    /// </summary>
    /// <param name="data"></param>
    /// <param name="filePath"></param>
    private bool MakeJson(object data, string filePath)
    {
        try
        {
            // jsonとして変換
            string json = JsonConvert.SerializeObject(data);
            using (StreamWriter wr = new StreamWriter(filePath, false))
            {
                // json変換した情報を書き込み
                wr.WriteLine(json);
            }
            return true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return false;
        }
    }

    public GameSaveData GetGameSaveData(bool isForce = false)
    {
        if (_gameSaveData == null || isForce == true)
        {
            if (Load<GameSaveData>(out _gameSaveData))
            {

            }
        }
        return _gameSaveData;
    }

    public bool IsTutorialFinished() 
    {
        if (_gameSaveData == null) return false;

        return _gameSaveData.IsTutorialFinished;
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