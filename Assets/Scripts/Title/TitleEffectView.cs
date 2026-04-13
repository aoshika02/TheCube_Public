using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TitleEffectView : MonoBehaviour
{
    private const float _startDelay = 2f;
    private Vector3 _initCameraRot = new(0, 0, 0);
    private Vector3 _startCameraRot = new(-45f, 0, 0);
    private const float _rotateDuration = 1f;
    private const float _minDuration = 4;
    private const float _maxDuration = 7;
    private const float _moveParDuration = 0.1f;
    [SerializeField] private List<TitleRoute> _titleRouteDatas = new List<TitleRoute>();
    [SerializeField] private int _maxPosY = 0;
    [SerializeField] private int _tilePosY = 0;
    [SerializeField] private int _minPosY = 0;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _effectRoot;
    private PlayerCube _playerCube;
    private Transform _playerCubeTransform;
    private readonly Dictionary<int, TitleRoute> _titleRouteDic = new Dictionary<int, TitleRoute>();
    private Queue<int> _indexQueue = new Queue<int>();
    private List<int> _indexList = new List<int>();
    public void Init(PlayerCube playerCube)
    {
        _playerCube = playerCube;
        _playerCubeTransform = _playerCube.GetParent(ParentType.Rotate);

        InitDict();
    }

    public void InitDict()
    {
        int key = 0;
        foreach (var titleRouteData in _titleRouteDatas)
        {
            if (_titleRouteDic.TryAdd(key, titleRouteData))
            {
                _indexList.Add(key);
                key++;
            }
        }
    }

    public void SetEffectRootActive(bool isActive)
    {
        _effectRoot.SetActive(isActive);
    }

    public void SetCameraActive(bool isActive)
    {
        _camera.gameObject.SetActive(isActive);
    }

    public void CameraRotInit()
    {
        _cameraTransform.rotation = Quaternion.Euler(_initCameraRot);
    }

    public async UniTask TitleEffectFlow(CancellationToken token)
    {
        try
        {
            _playerCube.gameObject.SetActive(false);
            await UniTask.WaitForSeconds(_startDelay, cancellationToken: token);
            await _cameraTransform
                .DORotate(_startCameraRot, _rotateDuration)
                .SetEase(Ease.InOutSine)
                .ToUniTask(cancellationToken: token);
            while (!token.IsCancellationRequested)
            {
                await UniTask.WaitForSeconds(UnityEngine.Random.Range(_minDuration, _maxDuration), cancellationToken: token);
                var data = GetTitleRouteData();
                if (data == null) continue;
                if (data.Transforms == null || data.Transforms.Length == 0) continue;
                await CubeEffectFlow(data, token);
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }
        finally
        {
            _playerCube.gameObject.SetActive(true);
            _cameraTransform.rotation = Quaternion.Euler(_initCameraRot);
        }
    }

    private async UniTask CubeEffectFlow(TitleRoute titleRoute, CancellationToken token)
    {
        try
        {
            var transforms = titleRoute.Transforms;
            var spawnPos = transforms[0].position;

            _playerCubeTransform.position = new Vector3(
                spawnPos.x,
                _maxPosY,
                spawnPos.z);

            _playerCube.gameObject.SetActive(true);

            await CubeMoveUtil.MoveYAsync(_playerCubeTransform, _tilePosY, MathF.Abs(_maxPosY - _tilePosY) * _moveParDuration, token);

            for (int i = 1; i < transforms.Length; i++)
            {
                Vector3 currentPos = transforms[i - 1].position;
                Vector3 nextPos = transforms[i].position;
                Vector3 dir = nextPos - currentPos;
                Vector2Int moveDir = new Vector2Int(
                    (int)dir.x,
                    (int)dir.z);

                Vector3 moveTarget = CubeMoveUtil.GetNextPos(_playerCubeTransform, moveDir);
                Quaternion targetRot = CubeMoveUtil.GetNextQuaternion(_playerCubeTransform, moveDir);
                await CubeMoveUtil.MoveFlowAsync(_playerCubeTransform, moveTarget, targetRot, _rotateDuration, token);
            }

            await CubeMoveUtil.MoveYAsync(_playerCubeTransform, _minPosY, MathF.Abs(_tilePosY - _minPosY) * _moveParDuration, token);
            _playerCube.gameObject.SetActive(false);
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }


    private TitleRoute GetTitleRouteData()
    {
        int index = RandomTypeBase.GetRandomType(_indexQueue, _indexList);

        if (_titleRouteDic.TryGetValue(index, out TitleRoute titleRouteData))
        {
            return titleRouteData;
        }
        return null;
    }
}
