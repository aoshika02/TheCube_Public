using R3;
public class StageSelectModel
{
    public Observable<StageSaveData> OnIndexUpdate => _onIndexUpdate;
    private Subject<StageSaveData> _onIndexUpdate = new Subject<StageSaveData>();
    public ReadOnlyReactiveProperty<int> StageID => _stageID;
    private ReactiveProperty<int> _stageID = new ReactiveProperty<int>(-1);
    private StageSaveManager _stageSaveManager;

    public int MinStageID => _minStageID;
    private int _minStageID = 0;
    public int MaxStageID => _maxStageID;
    private int _maxStageID = 10;

    public StageSelectModel(StageSaveManager stageSaveManager, int maxStageID, int currentID)
    {
        _maxStageID = maxStageID;
        _stageID = new ReactiveProperty<int>(currentID);
        _stageSaveManager = stageSaveManager;
    }

    public void AddStageID()
    {
        if (_stageID.Value >= _maxStageID)
        {
            return;
        }
        _stageID.Value++;
        SetStageID(_stageID.Value);
    }

    public void SubtractStageID()
    {
        if (_stageID.Value <= _minStageID)
        {
            return;
        }
        _stageID.Value--;
        SetStageID(_stageID.Value);
    }

    public void SetStageID(int stageID)
    {
        if (GetStageSaveData(stageID, out var stageSaveData))
        {
            _onIndexUpdate.OnNext(stageSaveData);
        }
    }

    public bool GetStageSaveData(int stageID, out StageSaveData stageSaveData)
    {
        if (_stageSaveManager.GetSaveData(stageID, out stageSaveData))
        {
            return true;
        }

        UnityEngine.Debug.LogWarning($"ステージセレクトデータ取得失敗: {stageID}");
        return false;
    }
}
