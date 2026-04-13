public class StageJumpUICommand : IInputUIDataCommand
{
    private SelectJumpModel _selectJumpModel;

    public StageJumpUICommand(SelectJumpModel selectJumpModel)
    {
        _selectJumpModel = selectJumpModel;
    }

    public InputUIMovableData GetInputUIMovableData()
    {
        var currentSelectID = _selectJumpModel.CurrentSelectID;
        var columnCount = _selectJumpModel.ColumnCount;
        var rowCount = _selectJumpModel.RowCount;
        var minSelectID = _selectJumpModel.MinSelectID;
        var maxSelectID = _selectJumpModel.MaxSelectID;
        return new InputUIMovableData(
            UpMovable: currentSelectID / columnCount > minSelectID,
            LeftMovable: currentSelectID % columnCount > 0,
            DownMovable: currentSelectID / columnCount < rowCount - 1,
            RightMovable: currentSelectID % columnCount < columnCount - 1 && currentSelectID + 1 <= maxSelectID);
    }
}
