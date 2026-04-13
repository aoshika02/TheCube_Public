using R3;
using UnityEngine;

public class SelectJumpModel
{
    public Observable<SelectJumpInfo> OnIndexUpdate => _onIndexUpdate;
    private Subject<SelectJumpInfo> _onIndexUpdate = new Subject<SelectJumpInfo>();

    private int _currentSelectID = 0;
    private int _minSelectID = 0;
    private int _maxSelectID = 0;
    private int _columnCount = 0;
    private int _rowCount = 0;

    public int CurrentSelectID => _currentSelectID;
    public int MinSelectID => _minSelectID;
    public int MaxSelectID => _maxSelectID;
    public int ColumnCount => _columnCount;
    public int RowCount => _rowCount;

    private StageSelectModel _stageSelectModel;

    public SelectJumpModel(int columnCount, int minSelectID, int maxSelectID, int currentSelectID, StageSelectModel stageSelectModel)
    {
        _columnCount = columnCount;
        _minSelectID = minSelectID;
        _maxSelectID = maxSelectID;
        _currentSelectID = currentSelectID;
        _rowCount = (maxSelectID - minSelectID) / columnCount + 1;
        _stageSelectModel = stageSelectModel;
    }

    public void ChangeSelectID(Vector2Int input)
    {
        if (input == Vector2Int.zero)
        {
            return;
        }

        var lastSelectID = _currentSelectID;
        if (input.x != 0)
        {
            var currentColumn = _currentSelectID % _columnCount;
            if (input.x > 0 && currentColumn < _columnCount - 1 && _currentSelectID + 1 <= _maxSelectID)
            {
                _currentSelectID++;
            }
            else if (input.x < 0 && currentColumn > 0 && _currentSelectID - 1 >= _minSelectID)
            {
                _currentSelectID--;
            }
        }
        else if (input.y != 0)
        {
            var currentRow = _currentSelectID / _columnCount;
            if (input.y > 0 && currentRow > 0 && _currentSelectID - _columnCount >= _minSelectID)
            {
                _currentSelectID -= _columnCount;
            }
            else if (input.y < 0 && currentRow < _rowCount - 1 && _currentSelectID + _columnCount <= _maxSelectID)
            {
                _currentSelectID += _columnCount;
            }
        }

        if (lastSelectID != _currentSelectID)
        {
            _onIndexUpdate.OnNext(new SelectJumpInfo(_currentSelectID, lastSelectID));
        }
    }

    public void OnJump()
    {
        _stageSelectModel.SetStageID(_currentSelectID);
    }
}

public record SelectJumpInfo
{
    public readonly int SelectID;
    public readonly int LastSelectID;

    public SelectJumpInfo(int selectID, int lastSelectID)
    {
        SelectID = selectID;
        LastSelectID = lastSelectID;
    }
}
