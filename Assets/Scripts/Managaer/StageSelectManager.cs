using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;
using Extension;
using System.Linq;

public class StageSelectManager : SingletonMonoBehaviour<StageSelectManager>
{
    public int StageSelectCount => _stageSelectCount;
    private int _stageSelectCount = 0;
    private int _minStageSelectCount = 0;
    private int _maxStageSelectCount = 10;
    private bool _isCall = false;
    private bool _isSelectCall = false;
    [SerializeField] private int _moveLength = 5;
    [SerializeField] private float _moveDuration = 0.5f;

    private SelectTileView _selectTileView;
    private StagePreview _stagePreview;
    private InputUIView _inputUIView;
    private PlayerCube _playerCube;
    private SelectCameraManager _selectCameraManager;
    private FadeManager _fadeManager;
    private StageSaveManager _stageSaveManager;
    private GameInputStateManager _gameInputStateManager;
    private Subject<ArrowType> _onInput = new Subject<ArrowType>();
    private Subject<ArrowType> _onMove = new Subject<ArrowType>();

    public async UniTask Init(bool isBackFromInGame, int startIndex = 0)
    {
        bool isExist =
                InputManager.Instance != null &&
                StagePreview.Instance != null &&
                SelectTileView.Instance != null &&
                InputUIView.Instance != null &&
                PlayerCube.Instance != null &&
                SelectCameraManager.Instance != null &&
                FadeManager.Instance != null &&
                StageSaveManager.Instance != null &&
                DataSetLoader.Instance != null &&
                SceneLoadManager.Instance != null &&
                SaveLoadManager.Instance != null &&
                GameInputStateManager.Instance != null;

        await UniTask.WaitUntil(() => isExist);

        _fadeManager = FadeManager.Instance;
        _selectTileView = SelectTileView.Instance;
        _inputUIView = InputUIView.Instance;
        _stagePreview = StagePreview.Instance;
        _stagePreview.Init();
        _stagePreview.SetCanvasPos(startIndex * _moveLength);
        _playerCube = PlayerCube.Instance;
        _playerCube.Clear(new Vector3(startIndex * _moveLength, 0, isBackFromInGame ? _stagePreview.GetCanvasPos().z : 0), Quaternion.identity);
        _inputUIView.InitDic();
        _inputUIView.UpdateActive(new InputUIActiveData(true, true, false, true));
        _maxStageSelectCount = DataSetLoader.Instance.GetTotalStageCount() - 1;
        _stageSaveManager = StageSaveManager.Instance;
        _gameInputStateManager = GameInputStateManager.Instance;
        var saveData = SaveLoadManager.Instance.GetGameSaveData(isBackFromInGame);
        if (saveData == null)
        {
            _stageSaveManager.Init(_minStageSelectCount, _maxStageSelectCount);
        }
        else
        {
            _stageSaveManager.Init(saveData.StageSaveDatas.ToList());
        }
        _selectCameraManager = SelectCameraManager.Instance;
        _selectCameraManager.Init(_playerCube, startIndex * _moveLength);
        _isCall = true;
        _isSelectCall = false;
        _stageSelectCount = startIndex;

        InputManager.Instance.KeyW.Subscribe(_ =>
        {
            if (_ != 1) return;
            _onInput.OnNext(ArrowType.Up);
        }).AddTo(this);

        InputManager.Instance.KeyS.Subscribe(_ =>
        {
            if (_ != 1) return;
            _onInput.OnNext(ArrowType.Down);
        }).AddTo(this);

        InputManager.Instance.KeyA.Subscribe(_ =>
        {
            if (_ != 1) return;
            _onInput.OnNext(ArrowType.Left);
        }).AddTo(this);

        InputManager.Instance.KeyD.Subscribe(_ =>
        {
            if (_ != 1) return;
            _onInput.OnNext(ArrowType.Right);
        }).AddTo(this);

        _onInput.Subscribe(x =>
        {
            if (_gameInputStateManager.CurrentInputState != GameInputState.Other) return;
            OnInput(x);
        }).AddTo(this);

        _onMove
            .StartWith(ArrowType.Zero)
            .Subscribe(x =>
            {
                UpdateStageIndex(x);
            }).AddTo(this);

        if (isBackFromInGame)
            _stagePreview.SetBounceScale(Vector3.zero);

        await _fadeManager.FadeIn();

        if (isBackFromInGame)
            await MoveToSelectView(true);

        _selectCameraManager.SetIsFollow(true);
        _gameInputStateManager.SetInputState(GameInputState.Other);
        _isCall = false;
        _isSelectCall = false;
    }

    private async void OnInput(ArrowType arrowType)
    {
        if (_isCall) return;
        _isCall = true;

        if (arrowType == ArrowType.Up)
        {
            await MoveToSelectView(false);
            await _fadeManager.FadeOut();
            await LoadStageSelectSceneAsync();
            _isCall = false;
            return;
        }

        if (arrowType == ArrowType.Up || arrowType == ArrowType.Down)
        {
            await _playerCube.UnMoveFlow(
                arrowType == ArrowType.Up ? Vector2Int.up : Vector2Int.down,
                false,
                destroyCancellationToken,
                _moveDuration);

            _isCall = false;
            return;
        }

        var moveDir = arrowType.Arrow2Vector2Int();
        bool isMove = false;

        if ((_minStageSelectCount >= _stageSelectCount && moveDir.x < 0) ||
            (_maxStageSelectCount <= _stageSelectCount && moveDir.x > 0))
        {
            _isCall = false;
            return;
        }

        await _stagePreview.ShowStagePreview(false);
        for (int i = 0; i < _moveLength; i++)
        {
            var result = await _playerCube.MoveAsync(
                moveDir,
                false,
                destroyCancellationToken,
                DirectionType.None,
                false,
                _moveDuration / _moveLength);
            if (result.IsSuccess)
            {
                if (isMove == false)
                {
                    isMove = true;
                }
            }
            var posX = _playerCube.GetPlayerPos().x;

            _stagePreview.SetCanvasPos(posX);
        }

        if (isMove)
            _onMove.OnNext(arrowType);
        else
            _isCall = false;
    }

    private async void UpdateStageIndex(ArrowType arrowType)
    {
        var moveDir = arrowType.Arrow2Vector2Int();
        if (moveDir.x != 0)
        {
            _stageSelectCount = _stageSelectCount + moveDir.x;
            if (_stageSelectCount < _minStageSelectCount)
            {
                _stageSelectCount = _minStageSelectCount;
            }
        }
        if (_stageSaveManager.GetSaveData(_stageSelectCount, out var stageSaveData))
        {
            Debug.Log($"ステージセレクトデータ取得: {_stageSelectCount}");
            _stagePreview.UpdatePreviewStage(stageSaveData);
        }
        else
        {
            Debug.LogWarning($"ステージセレクトデータ取得失敗: {_stageSelectCount}");
        }

        _selectTileView.SetStages(new StageSelectData
            (
            _stageSelectCount,
            _minStageSelectCount,
            _maxStageSelectCount,
            _moveLength
            ), moveDir.x);

        _inputUIView.UpdateMovable(new InputUIMovableData(
            true,
            _minStageSelectCount < _stageSelectCount,
            false,
            _maxStageSelectCount > _stageSelectCount));

        await _stagePreview.ShowStagePreview(true);
        _isCall = false;
    }
    public async UniTask LoadStageSelectSceneAsync()
    {
        _gameInputStateManager.SetInputState(GameInputState.None);
        await SceneLoadManager.Instance.LoadSceneAsync(_stageSelectCount);
    }

    private async UniTask MoveToSelectView(bool isShow)
    {
        if (_isSelectCall) return;
        _isSelectCall = true;
        var previewPos = _stagePreview.GetCanvasPos();
        Vector3[] path = new Vector3[3]
        {
            new Vector3(
                _stageSelectCount * _moveLength,
                0,
                isShow? previewPos.z:0),
            previewPos,
            new Vector3(
                _stageSelectCount * _moveLength,
                0,
                isShow? 0:previewPos.z)
        };
        await UniTask.WhenAll(
        _playerCube.PlayerCubeDoPath(path, isShow ? 0.25f : 0f),
        _stagePreview.BounceAnim(isShow, isShow ? 0f : 0.25f));
    }
}
