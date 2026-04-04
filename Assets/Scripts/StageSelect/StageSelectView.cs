using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;

public class StageSelectView : MonoBehaviour
{
    [SerializeField] private int _moveLength = 5;
    [SerializeField] private float _moveDuration = 0.5f;
    [SerializeField] private StagePreviewView _stagePreview;
    [SerializeField] private SelectTileView _selectTileView;
    [SerializeField] private SelectCameraManager _selectCameraManager;
    private TilePool _tilePool;
    private TileManager _tileManager;
    private DataSetLoader _dataLoader;
    private PlayerCube _playerCube;
    private FadeManager _fadeManager;
    private Transform _playerCubeTransform;
    private bool _isCallOnce = false;
    private float _tweenDelay = 0.25f;

    public void Initialized(TilePool tilePool, TileManager tileManager, DataSetLoader dataLoader, PlayerCube playerCube, FadeManager fadeManager, int stageID)
    {
        _isCallOnce = false;
        _tilePool = tilePool;
        _tileManager = tileManager;
        _dataLoader = dataLoader;
        _playerCube = playerCube;
        _fadeManager = fadeManager;
        _playerCubeTransform = _playerCube.GetParent(ParentType.Rotate);
        _selectCameraManager.Init(_playerCube, stageID * _moveLength);

        _selectTileView.Initialized(
            _dataLoader.GetTileDataSet(TileType.Normal),
            _dataLoader.GetFrameTexture(), _tilePool,
            0,
            _dataLoader.GetTotalStageCount() - 1,
            _moveLength,
            stageID);
    }

    public async UniTask UpdateStagePreviewAsync(int stageID, bool isClear, CancellationToken token)
    {
        await _stagePreview.ShowStagePreview(false, token: token);
        SetStagePreview(stageID, isClear);
        await MoveFlow(stageID * _moveLength > _playerCubeTransform.position.x ? Vector2Int.right : Vector2Int.left, token);
        await _stagePreview.ShowStagePreview(true, token: token);
    }

    public void SetStagePreview(int stageID, bool isClear)
    {
        var stageData = _dataLoader.GetStageDataSet(stageID);
        if (stageData != null)
        {
            _tilePool.ReleaseAll(UseType.Preview);
            _tileManager.Init(stageData.TileTypeData, UseType.Preview, false, "Preview");
            var data = stageData.TileTypeData;
            _stagePreview.SetCameraPos(data);
            _stagePreview.SetCanvasPos(stageID * _moveLength);
            _stagePreview.SetStageTitle(stageID + 1);
            _stagePreview.SetClear(isClear);
        }
    }

    public async UniTask UnMoveFlow(CancellationToken token)
    {
        await CubeMoveUtil.UnMoveFlow(_playerCubeTransform, Vector2Int.down, moveTime: _moveDuration,token: token);
    }

    public async UniTask MoveFlow(Vector2Int moveDir, CancellationToken token)
    {
        for (int i = 0; i < _moveLength; i++)
        {
            Vector3 moveTarget = CubeMoveUtil.GetNextPos(_playerCubeTransform, moveDir);
            Quaternion targetRot = CubeMoveUtil.GetNextQuaternion(_playerCubeTransform, moveDir);
            _selectTileView.Scroll(moveDir.x);
            await CubeMoveUtil.MoveFlowAsync(_playerCubeTransform, moveTarget, targetRot, _moveDuration / _moveLength, token);
        }
    }

    public async UniTask SceneChangeFlow(int stageID, bool isFromInGame, CancellationToken token)
    {
        if (_isCallOnce == false)
        {
            _isCallOnce = true;
            _playerCubeTransform.position = new Vector3(stageID * _moveLength, 0, 0);
            _playerCubeTransform.rotation = Quaternion.identity;
            _playerCube.gameObject.SetActive(true);
            _selectCameraManager.SetCameraPos(stageID * _moveLength);
            _selectCameraManager.SetIsFollow(true);
            _selectTileView.SpawnTile(stageID * _moveLength);
            await _fadeManager.FadeIn(token: token);
            return;
        }
        var previewPos = _stagePreview.GetCanvasPos();
        var targetPos = new Vector3(stageID * _moveLength, 0, isFromInGame ? previewPos.z : 0);

        if (isFromInGame)
        {
            _playerCubeTransform.position = targetPos;
            _playerCubeTransform.rotation = Quaternion.identity;
            _selectCameraManager.SetCameraPos(targetPos.x);
            _selectCameraManager.SetIsFollow(true);
            _selectTileView.SpawnTile(stageID * _moveLength);
            await _fadeManager.FadeIn(token: token);
        }

        Vector3[] path = new Vector3[3]
        {
            targetPos,
            previewPos,
            new Vector3(
                stageID * _moveLength,
                0,
                isFromInGame? 0:previewPos.z)
        };
        await UniTask.WhenAll(
        CubeDoPath(_playerCubeTransform,path, isFromInGame ? _tweenDelay : 0f, token: token),
        _stagePreview.BounceAnim(isFromInGame, isFromInGame ? 0f : _tweenDelay, token: token));
        if (isFromInGame == false)
        {
            await _fadeManager.FadeOut(token: token);
            _selectCameraManager.SetIsFollow(false);
            _tilePool.ReleaseAll();
        }
    }

    private async UniTask CubeDoPath(Transform target, Vector3[] path, float delay = 0.25f, float duration = 1f, CancellationToken token = default)
    {
        await UniTask.WaitForSeconds(delay, cancellationToken: token);
        await target.transform
           .DOPath(path, duration)
           .SetEase(Ease.InOutSine)
           .ToUniTask(cancellationToken: token);
    }

    public void SetCameraActive(bool isActive)
    {
        _selectCameraManager.SetCameraActive(isActive);
    }

    public void ShutDown()
    {
        SetCameraActive(false);
        _selectCameraManager.SetIsFollow(false);
        _tilePool.ReleaseAll();
        _selectTileView.ShutDown();
    }

    public void OnClear(int posX)
    {
        _selectTileView.InitScrollData(posX);
    }
}
