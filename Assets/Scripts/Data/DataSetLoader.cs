using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using VContainer;

public class DataSetLoader : MonoBehaviour
{
    [SerializeField] private TileInitDatas _tileInitDatas;
    [SerializeField] private StageDatas _stageDatas;
    private GameMasterData _gameMasterData;

    [Inject]
    public void Construct()
    {
        _gameMasterData = new GameMasterData(_tileInitDatas, _stageDatas);
    }

    public StageDataSet GetStageDataSet(int stageID)
    {
        if (_gameMasterData == null)
        {
            Debug.LogError("GameMasterData が初期化されていません");
            return null;
        }
        return _gameMasterData.GetStageDataSet(stageID);
    }

    public TileDataSet GetTileDataSet(TileType tileType)
    {
        if (_gameMasterData == null)
        {
            Debug.LogWarning("GameMasterData が初期化されていません");
            return null;
        }
        return _gameMasterData.GetTileDataSet(tileType);
    }

    public Texture2D GetFrameTexture()
    {
        if (_gameMasterData == null)
        {
            Debug.LogWarning("GameMasterData が初期化されていません");
            return null;
        }
        return _gameMasterData.FrameTexture;
    }

    public int GetMinStageID()
    {
        return _gameMasterData != null ? _gameMasterData.GetMinStageID() : 0;
    }

    public int GetMaxStageID()
    {
        return _gameMasterData != null ? _gameMasterData.GetMaxStageID() : 0;
    }
}

public class GameMasterData
{
    public readonly Texture2D FrameTexture;
    public readonly Dictionary<int, StageDataSet> StageDataSets = new Dictionary<int, StageDataSet>();
    public readonly Dictionary<TileType, TileDataSet> TileDataSets = new Dictionary<TileType, TileDataSet>();

    public GameMasterData(TileInitDatas tileInitDatas, StageDatas stageDatas)
    {
        FrameTexture = tileInitDatas.FrameTexture;

        foreach (var tileInitData in tileInitDatas.TileInitDataList)
        {
            TileDataSets.TryAdd(
                tileInitData.TileType,
                new TileDataSet(
                    tileInitData.TileType,
                    tileInitData.IconTexture,
                    tileInitData.BaseColor,
                    tileInitData.FrameColor,
                    tileInitData.IconColor
                    )
                );
        }

        foreach (var stageData in stageDatas.StageDataList)
        {
            var tileTypeData = MapLoader.Load(stageData.StageJson.text);
            StageDataSets.TryAdd(
                stageData.StageID,
                new StageDataSet(
                    stageData.StageID,
                    stageData.StartID,
                    tileTypeData
                    )
                );
        }

    }

    public StageDataSet GetStageDataSet(int stageID)
    {
        if (StageDataSets.TryGetValue(stageID, out var stageDataSet))
        {
            return stageDataSet;
        }
        else
        {
            Debug.LogWarning($"StageDataSet が見つかりません: {stageID}");
            return null;
        }
    }

    public TileDataSet GetTileDataSet(TileType tileType)
    {
        if (TileDataSets.TryGetValue(tileType, out var tileDataSet))
        {
            return tileDataSet;
        }
        else
        {
            Debug.LogWarning($"TileDataSet が見つかりません: {tileType}");
            return null;
        }
    }

    public int GetMinStageID()
    {
        if (StageDataSets.Count == 0)
        {
            Debug.LogWarning("StageDataSets が空です");
            return 0;
        }
        return StageDataSets.Keys.ToList().Min();
    }

    public int GetMaxStageID()
    {
        if (StageDataSets.Count == 0)
        {
            Debug.LogWarning("StageDataSets が空です");
            return 0;
        }
        return StageDataSets.Keys.ToList().Max();
    }
}

    public record StageDataSet
    {
        public readonly int StageID;
        public readonly int StartID;
        public ReadOnlyCollection<ReadOnlyCollection<TileType>> TileTypeData;

        public StageDataSet(int stageID, int startID, List<List<TileType>> tileTypeData)
        {
            StageID = stageID;
            StartID = startID;
            var readOnlyTileTypeData = new List<ReadOnlyCollection<TileType>>();
            foreach (var row in tileTypeData)
            {
                readOnlyTileTypeData.Add(new ReadOnlyCollection<TileType>(row));
            }
            TileTypeData = new ReadOnlyCollection<ReadOnlyCollection<TileType>>(readOnlyTileTypeData);
        }


    }

    public record TileDataSet
    {
        public readonly TileType TileType;
        public readonly Texture2D IconTexture;
        public readonly Color BaseColor;
        public readonly Color FrameColor;
        public readonly Color IconColor;

        public TileDataSet(TileType tileType, Texture2D iconTexture, Color baseColor, Color frameColor, Color iconColor)
        {
            TileType = tileType;
            IconTexture = iconTexture;
            BaseColor = baseColor;
            FrameColor = frameColor;
            IconColor = iconColor;
        }
    }