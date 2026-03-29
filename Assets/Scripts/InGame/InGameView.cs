using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using VContainer;

public class InGameView : MonoBehaviour
{
    private FadeManager _fadeManager;
    private TileManager _tileManager;
    private TilePool _tilePool;
    private PlayerCube _playerCube;
    private PlayerCamera _playerCameraMove;
    private float _spawnPosY = 11;
    private float _startPosY = 0;
    private Vector3 _startPos;
    private float _moveParDuration = 0.1f;

    [Inject]
    public void Construct(TilePool tilePool, TileManager tileManager, PlayerCube playerCube, PlayerCamera playerCameraMove, FadeManager fadeManager)
    {
        _tilePool = tilePool;
        _tileManager = tileManager;
        _playerCube = playerCube;
        _playerCameraMove = playerCameraMove;
        _fadeManager = fadeManager;
    }

    public async UniTask InitializeAsync(int startID, CancellationToken token)
    {
        TileObj tileObj = _tileManager.GetTileObj(startID);
        var startPos = new Vector3(tileObj.transform.position.x, _startPosY, tileObj.transform.position.z);
        _startPos = startPos;
        _playerCube.SetCubesPos(_startPos);
        await UniTask.Yield(token);
        _playerCube.InitTilePos();
        _playerCameraMove.SetPos(_startPos);
        _playerCube.SetCubesPos(_startPos);

        startPos.y = _spawnPosY;
        var cubeTransform = _playerCube.GetParent(ParentType.Rotate);
        cubeTransform.position = startPos;

        await _fadeManager.FadeIn(token: token);
        await CubeMoveUtil.MoveYAsync(cubeTransform, _startPosY, _moveParDuration * (_spawnPosY - _startPosY), token);
    }

    public async UniTask ResetAsync(CancellationToken token)
    {
        await _fadeManager.FadeOut(token: token);
        _playerCube.ResetAllCubeTile();
        _playerCube.SetCubesPos(_startPos);
        _playerCube.SetCubesRot(Quaternion.identity);
        await _fadeManager.FadeIn(token: token);
    }

    public async UniTask ShutDown(CancellationToken token)
    {
        await _fadeManager.FadeOut(token: token);
        _playerCube.ResetAllCubeTile();
        _playerCameraMove.SetCameraActive(false);
        _tilePool.ReleaseAll();
    }
}
