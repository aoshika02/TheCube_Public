public class InGameModel
{
    private DataSetLoader _dataSetLoader;
    private LoadManager _loadManager;
    private SaveManager _saveManager;
    private StageSaveManager _stageSaveManager;
    private TileManager _tileManager;
    private TileInfoModel _tileInfoModel;
    private StageSelectModel _stageSelectModel;

    public int StartID => _startId;
    private int _startId = 0;

    public InGameModel(
        DataSetLoader dataSetLoader,
        LoadManager loadManager,
        SaveManager saveManager,
        StageSaveManager stageSaveManager,
        TileManager tileManager,
        TileInfoModel tileInfoModel,
        StageSelectModel stageSelectModel)
    {
        _dataSetLoader = dataSetLoader;
        _loadManager = loadManager;
        _saveManager = saveManager;
        _stageSaveManager = stageSaveManager;
        _tileManager = tileManager;
        _tileInfoModel = tileInfoModel;
        _stageSelectModel = stageSelectModel;
    }

    public void Initialize()
    {
        var stageID = _stageSelectModel.StageID.CurrentValue;
        StageDataSet stageData = _dataSetLoader.GetStageDataSet(stageID);
        _tileInfoModel.Initialize(stageData);
        _tileManager.Init(stageData.TileTypeData);
        _startId = stageData.StartID;
    }

    public void Reset()
    {
        _tileManager.ClearDetachmentType();
    }

    public void ShutDown()
    {
        _tileManager.ClearActiveTile();
        _stageSaveManager.SetSaveData(_stageSelectModel.StageID.CurrentValue, true);
        if (_saveManager.Save(_stageSelectModel.StageID.CurrentValue))
        {
            _loadManager.ClearCache();
        }
    }
}
