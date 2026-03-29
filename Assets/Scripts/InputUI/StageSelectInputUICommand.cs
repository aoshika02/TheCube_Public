public class StageSelectInputUICommand : IInputUIDataCommand
{
    private StageSelectModel _model;

    public StageSelectInputUICommand(StageSelectModel model)
    {
        _model = model;
    }
    public InputUIMovableData GetInputUIMovableData()
    {
        return new InputUIMovableData(
            true,
            _model.MinStageID < _model.StageID.CurrentValue,
            false,
            _model.MaxStageID > _model.StageID.CurrentValue
            );
    }
}
