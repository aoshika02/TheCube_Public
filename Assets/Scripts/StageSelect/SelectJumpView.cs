using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class SelectJumpView : MonoBehaviour
{
    [SerializeField] private GameObject _selectJumpObjPrefab;
    private GenericObjectPool<SelectJumpObj> _selectJumpObjPool;
    private Dictionary<int, SelectJumpObj> _selectJumpDict = new Dictionary<int, SelectJumpObj>();

    [SerializeField] private Color _selectColor;
    [SerializeField] private Color _unselectColor;
    [SerializeField] private Transform _jumpObjParent;
    [SerializeField] private GridLayoutGroup _gridLayoutGroup;

    [Inject]
    public void Construct()
    {
        _selectJumpObjPool = new GenericObjectPool<SelectJumpObj>(_selectJumpObjPrefab, transform);
    }

    public void Initialized(int stageCount, int currentStageCount = 0)
    {
        for (int i = 0; i < stageCount; i++)
        {
            var selectJumpObj = _selectJumpObjPool.Get();
            selectJumpObj.transform.SetParent(_jumpObjParent);
            selectJumpObj.SetStageID(i);
            selectJumpObj.SetColor(currentStageCount == i ? _selectColor : _unselectColor);
            _selectJumpDict[i] = selectJumpObj;
        }
    }

    public void UpdateSelectJumpColor(SelectJumpInfo selectJumpInfo) 
    {
        if(_selectJumpDict.TryGetValue(selectJumpInfo.SelectID, out var selectJumpObj))
        {
            selectJumpObj.SetColor(_selectColor);
        }

        if(_selectJumpDict.TryGetValue(selectJumpInfo.LastSelectID, out var lastSelectJumpObj))
        {
            lastSelectJumpObj.SetColor(_unselectColor);
        }
    }

    public int GetColumnCount()
    {
        return _gridLayoutGroup.constraintCount;
    }
}
