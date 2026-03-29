using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveLoadUtil
{
    public static bool DirectoryCheck(string folderPath)
    {
        try
        {
            // フォルダがなければ作成
            Directory.CreateDirectory(folderPath);
            return true;
        }
        catch
        {
            Debug.LogError("フォルダの確認＆作成に失敗");
            return false;
        }
    }

    public static string GetBasePath()
    {
#if UNITY_EDITOR
        return Application.dataPath;
#else
    return Application.persistentDataPath;
#endif
    }

    public static string MakePath(List<string> paths)
    {
        return string.Join("/", paths);
    }

    public static bool Load<T>(string path, out T outData) where T : class
    {
        try
        {
            using (StreamReader rd = new StreamReader(path))
            {
                string json = rd.ReadToEnd();
                outData = JsonConvert.DeserializeObject<T>(json);
                return true;
            }
        }
        catch
        {
            Debug.LogWarning($"セーブデータの読み込みに失敗しました log : {path} is not found");
            outData = null;
            return false;
        }
    }

    public static bool MakeJson(object data, string filePath)
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

    public static bool TryDelete(string path)
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
}