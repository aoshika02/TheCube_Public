using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

public class PlayerCube : MonoBehaviour
{
    [SerializeField] private Transform _rotateParent;
    [SerializeField] private Transform _unRotateParent;
    [SerializeField] private Transform _reverseRotateParent;
    [SerializeField] private Color _defaultColor;           //006BFF
    [SerializeField] private Color _lastAttachmentColor;    //FF05AA
    [SerializeField] private Color _detachmentColor;        //FF1700
    [SerializeField] private List<CubeFace> _playerFaces = new List<CubeFace>();
    public DirectionType CurrentBottomDir => DirectionUtil.GetBottomDirection(_rotateParent.rotation);

    private readonly Dictionary<DirectionType, CubeFace> _faceDict = new Dictionary<DirectionType, CubeFace>();
    private readonly Dictionary<ParentType, Transform> _parentDict = new Dictionary<ParentType, Transform>();
    private readonly Dictionary<ColorType, Color> _colorDict = new Dictionary<ColorType, Color>();

    [Inject]
    public void Construct()
    {
        Init();
    }

    private void Init()
    {
        InitTilePos();
        InitDict();
    }

    public void InitTilePos()
    {
        _playerFaces.ForEach(x => x.Init());
    }

    #region Dictionaryメソッド
    private void InitDict()
    {
        _colorDict[ColorType.Default] = _defaultColor;
        _colorDict[ColorType.LastAttachment] = _lastAttachmentColor;
        _colorDict[ColorType.Detachment] = _detachmentColor;
        _parentDict[ParentType.Rotate] = _rotateParent;
        _parentDict[ParentType.UnRotate] = _unRotateParent;
        _parentDict[ParentType.ReverseRotate] = _reverseRotateParent;

        foreach (var face in _playerFaces)
        {
            _faceDict.TryAdd(face.DirectionType, face);
        }
    }

    public CubeFace GetCubeFace(DirectionType type)
    {
        if (_faceDict.TryGetValue(type, out var face))
        {
            return face;
        }
        return null;
    }

    public Transform GetParent(ParentType type)
    {
        if (_parentDict.TryGetValue(type, out var parent))
        {
            return parent;
        }
        return null;
    }

    private Color GetColor(ColorType type)
    {
        if (_colorDict.TryGetValue(type, out var color))
        {
            return color;
        }
        return Color.white;
    }

    #endregion

    #region 色変更関係メソッド

    public void ChangeCubeBaseColor(List<MoveInfo> moveInfos, int index, ColorType colorType)
    {

        if (moveInfos.Count > index)
        {
            var moveInfo = moveInfos[moveInfos.Count - 1 - index];
            if (moveInfo != null)
                ChangeCubeBaseColor(moveInfo.DetachmentType, colorType);
        }
    }

    public void ChangeCubeBaseColor(DirectionType type, ColorType colorType)
    {
        var color = GetColor(colorType);
        _playerFaces.FirstOrDefault(x => x.DirectionType == type)?.SetColor(color);
    }
    #endregion

    #region 面離脱メソッド
    public void Detachments(List<DirectionType> detachTypes)
    {
        foreach (var directionType in detachTypes)
        {
            Detachment(ParentType.Rotate, directionType);
        }
    }

    public void Detachment(ParentType parentType, DirectionType directionType)
    {
        var parent = GetParent(parentType);
        var cubeObj = GetCubeFace(directionType)?.gameObject;
        var t = cubeObj.transform;
        Vector3 pos = t.position;
        Quaternion rot = t.rotation;

        t.SetParent(parent);

        t.position = pos;
        t.rotation = rot;
    }
    #endregion

    public void SetCubesPos(Vector3 pos)
    {
        _rotateParent.position = pos;
        _reverseRotateParent.position = pos;
        _unRotateParent.position = pos;
    }

    public void SetCubesRot(Quaternion rot)
    {
        _rotateParent.rotation = rot;
        _reverseRotateParent.rotation = rot;
        _unRotateParent.rotation = rot;
    }

    public void ResetAllCubeTile()
    {
        // 全ての面の親子関係をリセットし、色もデフォルトに戻す
        _playerFaces.ForEach(x =>
        {
            x.transform.SetParent(_rotateParent);
            x.SetColor(_defaultColor);
            x.ResetTransform();
        });
    }
}