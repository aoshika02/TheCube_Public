using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    private TileManager _tileManager;
    private PlayerCube _playerCube;
    private PlayerMoveInput _playerMoveInput;
    private PlayerCameraMove _playerCameraMove;
    private InputUIView _inputUIView;
    private FadeManager _fadeManager;
    private DataSetLoader _dataSetLoader;
    private GameInputStateManager _gameInputStateManager;
    private TileInfoSystem _tileInfoSystem;
    private TutorialManager _tutorialManager;
    private SaveLoadManager _saveLoadManager;
    private bool _isFlowing = false;
    private int _startId = 0;
    private bool _isMoveable = true;
    [SerializeField] private int _stageIndex;
    private float _spawnPosY = 11;
    private Vector3 _startPos;

    public async UniTask Init(int stageID)
    {

        bool isExist = InputManager.Instance != null &&
                TileManager.Instance != null &&
                TilePool.Instance != null &&
                PlayerCube.Instance != null &&
                PlayerMoveInput.Instance != null &&
                PlayerCameraMove.Instance != null &&
                FadeManager.Instance != null &&
                InputUIView.Instance != null &&
                DataSetLoader.Instance != null &&
                GameInputStateManager.Instance != null &&
                TileInfoSystem.Instance != null &&
                TutorialManager.Instance != null &&
                SaveLoadManager.Instance != null &&
                IconUIManager.Instance != null;

        await UniTask.WaitUntil(() => isExist);


        _isMoveable = false;
        _isFlowing = false;

        _dataSetLoader = DataSetLoader.Instance;
        StageDataSet stageData = _dataSetLoader.GetStageDataSet(stageID);
        _tileInfoSystem = TileInfoSystem.Instance;
        _tileInfoSystem.SetInfos(stageData);
        await UniTask.Yield();
        _tileInfoSystem.RebuildLayout();
        _tileInfoSystem.SwitchInfoViewActive();
        _tileManager = TileManager.Instance;
        _tileManager.Init(stageData.TileTypeData);
        _playerCube = PlayerCube.Instance;
        _playerMoveInput = PlayerMoveInput.Instance;
        _playerCameraMove = PlayerCameraMove.Instance;
        _fadeManager = FadeManager.Instance;
        _inputUIView = InputUIView.Instance;
        _inputUIView.InitDic();
        _gameInputStateManager = GameInputStateManager.Instance;
        _stageIndex = stageID;
        _startId = stageData.StartID;
        _saveLoadManager = SaveLoadManager.Instance;
        _tutorialManager = TutorialManager.Instance;
        _tutorialManager.Init(_saveLoadManager.IsTutorialFinished());
        OnReset(true);

        IconUIManager.Instance.Init();

        _gameInputStateManager.OnInputStateChanged
            .StartWith(_gameInputStateManager.CurrentInputState)
            .Subscribe(state =>
            {
                Debug.Log($"InputState: {state}");
                if (state == GameInputState.Other)
                {
                    _inputUIView.UpdateMovable(MakeInputUIMovableData());
                    return;
                }
                _inputUIView.UpdateMovable(new InputUIMovableData(false, false, false, false));
            }).AddTo(this);

        _playerMoveInput.OnMoveInput.Subscribe(x =>
        {
            if (!_isMoveable) return;
            if (_gameInputStateManager.CurrentInputState != GameInputState.Other) return;
            OnMove(x).Forget();
        }).AddTo(this);

        _playerCameraMove.CameraAngleY
            .Subscribe(angleY =>
            {
                _inputUIView.UpdateAngle(angleY);
            }).AddTo(this);

        InputManager.Instance.KeyQ.Subscribe(x =>
        {
            if (x != 1) return;
            if (_gameInputStateManager.CurrentInputState != GameInputState.Other) return;
            LoadStageSelect().Forget();
        }).AddTo(this);

        InputManager.Instance.KeyR.Subscribe(x =>
        {
            if (x != 1) return;
            if (_gameInputStateManager.CurrentInputState != GameInputState.Other) return;
            OnResetAsync().Forget();
        }).AddTo(this);

        InputManager.Instance.KeyE.Subscribe(x =>
        {
            if (x != 1) return;
            if (_gameInputStateManager.CurrentInputState != GameInputState.Other) return;
            _tileInfoSystem.SwitchInfoViewActive();
        }).AddTo(this);

        _playerCameraMove.SetFollow(false);
        _playerCameraMove.SetPos(_startPos);
        await _fadeManager.FadeIn();
        await _tutorialManager.TutorialFlow();
        await _playerCube.MoveYAsync(_startPos.y, 0.1f * (_spawnPosY - _startPos.y));
        await UniTask.Yield();
        _playerCube.Init();
        await UniTask.Yield();
        _gameInputStateManager.SetInputState(GameInputState.Other);
        _playerCameraMove.SetFollow(true);
        _isMoveable = true;
    }

    private async UniTask OnMove(Vector2Int moveDir)
    {
        if (_isFlowing) return;
        _isFlowing = true;
        _gameInputStateManager.SetInputState(GameInputState.Moving);
        var curentPos = _playerCube.GetPlayerPos();
        var movedPos = _playerCube.PlayerMovedPos(moveDir);
        var currentTileType = _tileManager.GetTileType(_playerCube.GetPlayerPos());
        var targetTileType = _tileManager.GetTileType(movedPos);
        var detachment = DetachmentCheck(currentTileType);
        if (targetTileType == TileType.None)
        {
            Debug.Log("移動先にタイルがありません");
            await _playerCube.UnMoveFlow(moveDir, detachment, destroyCancellationToken);
            _isFlowing = false;
            _gameInputStateManager.SetInputState(GameInputState.Other);
            return;
        }

        var targetType = _tileManager.GetDetachmentType(movedPos);
        var result = await _playerCube.MoveAsync(moveDir, detachment, destroyCancellationToken, targetType);
        if (result.IsSuccess)
        {
            _tileManager.SetDetachmentType(curentPos, result.DetachmentType);
            if (targetTileType == TileType.Goal)
            {
                // ゴール処理
                Debug.Log("ゴールしました！");
                await _fadeManager.FadeOut();
                StageSaveManager.Instance.SetSaveData(_stageIndex, true);
                _saveLoadManager.Save(_stageIndex);
                await LoadStageSelect();
                return;
            }
            await TileEvent(targetTileType);
        }
        _isFlowing = false;
        _gameInputStateManager.SetInputState(GameInputState.Other);
    }

    private InputUIMovableData MakeInputUIMovableData()
    {
        return new InputUIMovableData(
            _playerCube.IsMovable(Vector2Int.up) && IsMovable(Vector2Int.up),
            _playerCube.IsMovable(Vector2Int.left) && IsMovable(Vector2Int.left),
            _playerCube.IsMovable(Vector2Int.down) && IsMovable(Vector2Int.down),
            _playerCube.IsMovable(Vector2Int.right) && IsMovable(Vector2Int.right)
            );
    }

    private void OnReset(bool isStart = false, bool isResetBases = false)
    {
        _isMoveable = false;
        var tileObj = _tileManager.GetTileObj(_startId);
        var startPos = new Vector3(tileObj.transform.position.x, _playerCube.transform.position.y, tileObj.transform.position.z);
        _playerCube.Clear(startPos, Quaternion.identity, isResetBases);

        if (isStart)
        {
            _startPos = startPos;
            startPos.y = _spawnPosY;
            _playerCube.SetPos(startPos);
            Debug.Log($"{_startId}:{_startPos}:{startPos}");
        }

        _isMoveable = true;
    }

    private async UniTask OnResetAsync()
    {
        _gameInputStateManager.SetInputState(GameInputState.None);
        await _fadeManager.FadeOut();
        OnReset(false, true);
        _tileManager.ClearDetachmentType();
        await _fadeManager.FadeIn();
        _gameInputStateManager.SetInputState(GameInputState.Other);
    }

    private async UniTask TileEvent(TileType tileType)
    {
        if (tileType == TileType.Reset)
        {
            _tileManager.ClearDetachmentType();
            await _playerCube.ReverseMoveAsync(destroyCancellationToken);
            return;
        }

        if (tileType.IsArrow())
        {
            var moveDir = tileType.ToVector2Int();

            if (moveDir != Vector2Int.zero)
            {
                if (_tileManager.GetCornerPos(_playerCube.GetPlayerPos(), moveDir, out var cornerPos))
                {
                    await _playerCube.AccelMoveFlow(cornerPos);
                    var targetTileType = _tileManager.GetTileType(cornerPos);
                    await TileEvent(targetTileType);
                }
            }
            return;
        }

        if (tileType.IsJump())
        {
            var moveDir = tileType.ToVector2Int();
            if (moveDir != Vector2Int.zero)
            {
                var jumpPos = _playerCube.PlayerMovedPos(moveDir * 2);
                var targetTileObj = _tileManager.GetTileObj(jumpPos);

                if (targetTileObj == null)
                {
                    _playerCameraMove.SetFollow(false);
                }

                await _playerCube.JumpFlow(jumpPos);

                if (targetTileObj != null)
                {
                    await TileEvent(targetTileObj.TileType);
                    return;
                }
                await _playerCube.MoveYAsync(-_spawnPosY);
                await OnResetAsync();
            }
            return;
        }

        if (tileType == TileType.Warp)
        {
            if (_tileManager.GetWarpPos(_playerCube.GetPlayerPos(), tileType, out var warpPos))
            {
                await _playerCube.WarpFlow(warpPos);
            }
        }

    }

    private bool DetachmentCheck(TileType tileType)
    {
        return tileType != TileType.Skip;
    }

    public bool IsMovable(Vector2Int moveDir)
    {
        var movedPos = _playerCube.PlayerMovedPos(moveDir);
        var targetTileType = _tileManager.GetTileType(movedPos);
        return targetTileType != TileType.None;
    }

    public async UniTask LoadStageSelect()
    {
        _gameInputStateManager.SetInputState(GameInputState.None);
        await SceneLoadManager.Instance.LoadStageSelectAsync(_stageIndex, true);
    }
}

[Serializable]
public record TileData
{
    public List<List<TileType>> TileTypes;
}