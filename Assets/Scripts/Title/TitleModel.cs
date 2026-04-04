public class TitleModel 
{
    private SaveManager _saveManager;
    private LoadManager _loadManager;
    private StageSaveManager _stageSaveManager;
    private StageSelectModel _stageSelectModel;
    private TutorialManager _tutorialManager;
    private TileInfoModel _tileInfoModel;
    private DialogModel _dialogModel;

    public TitleModel(SaveManager saveManager, LoadManager loadManager, StageSaveManager stageSaveManager, StageSelectModel stageSelectModel, TutorialManager tutorialManager, TileInfoModel tileInfoModel, DialogModel dialogModel)
    {
        _saveManager = saveManager;
        _loadManager = loadManager;
        _stageSaveManager = stageSaveManager;
        _stageSelectModel = stageSelectModel;
        _tutorialManager = tutorialManager;
        _tileInfoModel = tileInfoModel;
        _dialogModel = dialogModel;
    }

    public void DeleteSaveData()
    {
        if (_saveManager.DeleteSaveData())
        {
            _stageSelectModel.ClearID();
            _tutorialManager.SetTutorialFinished(false);
            _tileInfoModel.ClearVerifiedTypes();
            _loadManager.ClearCache();
            _stageSaveManager.ClearSaveData();
            _dialogModel.AddDialog(DialogEventType.DeleteSaveData);
        }
    }
}
