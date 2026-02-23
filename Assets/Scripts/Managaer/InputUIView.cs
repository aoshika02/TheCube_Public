using System.Collections.Generic;
using UnityEngine;
using System;
using Extension;

public class InputUIView : SingletonMonoBehaviour<InputUIView>
{
    [SerializeField] private RectTransform _inputRoot;
    [SerializeField] private List<ArrowObjData> _arrowDatas;
    private Dictionary<ArrowType, ArrowObj> _arrowObjDict = new Dictionary<ArrowType, ArrowObj>();
    private bool _isInit = false;
    protected override void Awake()
    {
        if (!CheckInstance()) return;
        _isInit = false;
    }

    public void InitDic()
    {
        if (_isInit) return;
        _isInit = true;
        foreach (var data in _arrowDatas)
        {
            if (_arrowObjDict.TryAdd(data.ArrowType, data.ArrowObj) == false)
            {
                Debug.LogWarning($"同じArrowTypeが登録されています -> {data.ArrowType}");
            }
        }
    }

    public void UpdateActive(InputUIActiveData activeData)
    {
        if(GetArrowObj(ArrowType.Up, out var upArrow))
        {
            upArrow.gameObject.SetActive(activeData.UpActive);
        }

        if (GetArrowObj(ArrowType.Left, out var leftArrow))
        {
            leftArrow.gameObject.SetActive(activeData.LeftActive);
        }

        if (GetArrowObj(ArrowType.Down, out var downArrow))
        {
            downArrow.gameObject.SetActive(activeData.DownActive);
        }

        if (GetArrowObj(ArrowType.Right, out var rightArrow))
        {
            rightArrow.gameObject.SetActive(activeData.RightActive);
        }
    }
   
    public void UpdateMovable(InputUIMovableData inputUIMovableData)
    {
        if (GetArrowObj(ArrowType.Up, out var upArrow))
        {
            upArrow.SetMovable(inputUIMovableData.UpMovable);
        }

        if (GetArrowObj(ArrowType.Left, out var leftArrow))
        {
            leftArrow.SetMovable(inputUIMovableData.LeftMovable);
        }

        if (GetArrowObj(ArrowType.Down, out var downArrow))
        {
            downArrow.SetMovable(inputUIMovableData.DownMovable);
        }

        if (GetArrowObj(ArrowType.Right, out var rightArrow))
        {
            rightArrow.SetMovable(inputUIMovableData.RightMovable);
        }
    }

    public void UpdateAngle(float angleY)
    {
        var euler = _inputRoot.localEulerAngles;
        euler.z = angleY;
        _inputRoot.localEulerAngles = euler;

        foreach (var data in _arrowDatas)
        {
            if (data != null && data.ArrowObj != null)
            {
                data.ArrowObj.SetAngle(-angleY);
            }
        }
    }
    
    private bool GetArrowObj(ArrowType arrowType,out ArrowObj result)
    {
        if (_arrowObjDict.TryGetValue(arrowType, out var arrowObj))
        {
            result = arrowObj;
            return true;
        }
        Debug.LogWarning($"ArrowObjが見つかりません -> {arrowType}");
        result = null;
        return false;
    }
}

[Serializable]
public class ArrowObjData
{
    public ArrowType ArrowType;
    public ArrowObj ArrowObj;
}

public record InputUIActiveData
{
    public bool UpActive;
    public bool LeftActive;
    public bool DownActive;
    public bool RightActive;
    public InputUIActiveData(
        bool UpActive,
        bool LeftActive,
        bool DownActive,
        bool RightActive)
    {
        this.UpActive = UpActive;
        this.LeftActive = LeftActive;
        this.DownActive = DownActive;
        this.RightActive = RightActive;
    }
}
public record InputUIMovableData
{
    public bool UpMovable;
    public bool LeftMovable;
    public bool DownMovable;
    public bool RightMovable;
    public InputUIMovableData(
        bool UpMovable,
        bool LeftMovable,
        bool DownMovable,
        bool RightMovable)
    {
        this.UpMovable = UpMovable;
        this.LeftMovable = LeftMovable;
        this.DownMovable = DownMovable;
        this.RightMovable = RightMovable;
    }
}
