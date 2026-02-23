using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TitleEffectManager : SingletonMonoBehaviour<TitleEffectManager>
{
    [SerializeField] private List<TitleRouteData> _titleRouteDatas = new List<TitleRouteData>();
    [SerializeField] private int _maxPosY = 0;
    [SerializeField] private int _tilePosY = 0;
    [SerializeField] private int _minPosY = 0;
    [SerializeField] private PlayerCube _playerCube;
    [SerializeField] private Transform _cameraTransform;

    private const float _startDelay = 2f;
    private Vector3 _startCameraRot = new(-45f, 0, 0);
    private const float _rotateDuration = 1f;
    private const float _minDuration = 4;
    private const float _maxDuration = 7;
    private const float _moveParDuration = 0.1f;

    private readonly Dictionary<int, TitleRouteData> _titleRouteDic = new Dictionary<int, TitleRouteData>();

    private Queue<int> _indexQueue = new Queue<int>();
    private List<int> _indexList = new List<int>();
    protected override void Awake()
    {
        if (CheckInstance() == false) return;
        int key = 0;
        foreach (var titleRouteData in _titleRouteDatas)
        {
            if (_titleRouteDic.TryAdd(key, titleRouteData))
            {
                _indexList.Add(key);
                key++;
            }
        }
        _playerCube.gameObject.SetActive(false);
    }

    private void Start()
    {
        TitleEffectFlow(destroyCancellationToken).Forget();
    }

    private async UniTask TitleEffectFlow(CancellationToken token)
    {
        try
        {
            await UniTask.WaitForSeconds(_startDelay, cancellationToken: token);
            await _cameraTransform.DORotate(_startCameraRot, _rotateDuration).ToUniTask(cancellationToken: token);
            while (!token.IsCancellationRequested)
            {
                await UniTask.WaitForSeconds(UnityEngine.Random.Range(_minDuration, _maxDuration), cancellationToken: token);
                await CubeEffectFlow(token);
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }

    private async UniTask CubeEffectFlow(CancellationToken token)
    {
        var data = GetTitleRouteData();
        if (data == null) return;
        if (data.Transforms == null || data.Transforms.Length == 0) return;

        try
        {
            var transforms = data.Transforms;
            var spawnPos = transforms[0].position;
            _playerCube.SetPos(new Vector3(
                spawnPos.x,
                _maxPosY,
                spawnPos.z));

            _playerCube.gameObject.SetActive(true);

            await _playerCube.MoveYAsync(_tilePosY, _maxPosY * _moveParDuration);

            for (int i = 1; i < transforms.Length; i++)
            {
                Vector3 currentPos = transforms[i - 1].position;
                Vector3 nextPos = transforms[i].position;
                Vector3 dir = nextPos - currentPos;
                Vector2Int moveDir = new Vector2Int(
                    (int)dir.x,
                    (int)dir.z);
                Debug.Log($"{dir}:{moveDir}");

                await _playerCube.MoveAsync(moveDir, false, token, isInGame: false);
            }

            await _playerCube.MoveYAsync(_minPosY, (_tilePosY - _minPosY) * _moveParDuration);
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }

    private TitleRouteData GetTitleRouteData()
    {
        int index = RandomTypeBase.GetRandomType(_indexQueue, _indexList);

        if (_titleRouteDic.TryGetValue(index, out TitleRouteData titleRouteData))
        {
            return titleRouteData;
        }
        return null;
    }
}

[Serializable]
public class TitleRouteData
{
    public Transform[] Transforms;
}
