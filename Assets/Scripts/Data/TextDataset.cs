using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using UnityEngine;

public class TextDataset : MonoBehaviour
{
    [SerializeField] private TextAsset _dialogJson;

    private readonly Dictionary<DialogEventType, TextData> _textDataDict = new Dictionary<DialogEventType, TextData>();

    private void Start()
    {
        Init(_dialogJson);
    }

    public void Init(TextAsset dialogJson)
    {
        try
        {
            TextDatas textDatas = JsonConvert.DeserializeObject<TextDatas>(dialogJson.text);

            foreach (var data in textDatas.TextDataList)
            {
                if (_textDataDict.TryAdd(data.DialogEventType, data) == false)
                {
                    Debug.LogWarning($"DialogEventType {data.DialogEventType} が重複しています。");
                }
            }

        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"TextDataset のデシリアライズに失敗しました: {e.Message}");
            return;
        }
    }

    public string GetText(DialogEventType dialogEventType)
    {
        if (_textDataDict.TryGetValue(dialogEventType, out TextData textData))
        {
            return textData.Message;
        }
        else
        {
            Debug.LogWarning($"DialogEventType {dialogEventType} に対応するテキストが見つかりません。");
            return string.Empty;
        }
    }

}

public class TextDatas
{
    public List<TextData> TextDataList;
}

public class TextData
{
    [JsonConverter(typeof(StringEnumConverter))]
    public DialogEventType DialogEventType;
    public string Message;
}