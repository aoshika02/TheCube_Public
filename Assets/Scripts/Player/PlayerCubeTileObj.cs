using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PlayerCube : SingletonMonoBehaviour<PlayerCube>
{
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _lastAttachmentColor;
    [SerializeField] private Color _detachmentColor;
    [SerializeField] private List<PlayerCubeData> _playerCubeDatas = new List<PlayerCubeData>();
    public void Init()
    {
        _playerCubeDatas.ForEach(x => x.CubeTile.Init());
    }

    #region 色変更関係メソッド

    private void ChangeCubeBaseColor(int index, Color color)
    {
        if (_moveDatas.Count > index)
        {
            var moveData = _moveDatas[_moveDatas.Count - 1 - index];
            if (moveData != null)
                ChangeCubeBaseColor(moveData.DetachmentType, color);
        }
    }

    private void ChangeCubeBaseColor(DirectionType type, Color color)
    {
        PlayerCubeData targetCube = null;
        targetCube = _playerCubeDatas.FirstOrDefault(x => x.DirectionType == type);
        if (targetCube != null)
        {
            targetCube.CubeTile.SetColor(color);
        }
    }

    #endregion

    #region 面離脱メソッド

    private void Detachment(Transform parent, DirectionType bottomType)
    {
        var bottomObj = _playerCubeDatas.FirstOrDefault(x => x.DirectionType == bottomType);
        if (bottomObj != null)
        {
            Detachment(bottomObj.CubeTile.gameObject, parent);
            Debug.Log(bottomType);
            return;
        }
    }

    private void Detachment(GameObject cubeObj, Transform parent)
    {
        var t = cubeObj.transform;
        Vector3 pos = t.position;
        Quaternion rot = t.rotation;

        t.SetParent(parent);

        t.position = pos;
        t.rotation = rot;
    }

    private void ResetAllCubeTile()
    {
        _playerCubeDatas.ForEach(x =>
        {
            Detachment(x.CubeTile.gameObject, _rotateParent);
            x.CubeTile.ResetTransform();
            x.CubeTile.SetColor(_defaultColor);
        });
    }
    #endregion

    private PlayerCubeData GetCubeTile(DirectionType type)
    {
        return _playerCubeDatas.FirstOrDefault(x => x.DirectionType == type);
    }
}

[Serializable]
public class PlayerCubeData
{
    public DirectionType DirectionType;
    public CubeTile CubeTile;
}