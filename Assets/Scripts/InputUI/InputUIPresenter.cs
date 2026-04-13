using UnityEngine;
using R3;
using VContainer;

public class InputUIPresenter : MonoBehaviour
{
    private GameStateManager _gameStateManager;
    private PlayerCamera _playerCameraMove;
    private InputUIModel _inputUIModel;
    private InputUIDataLocator _inputUIDataLocator;
    [SerializeField] private InputUIView _inputUIView;

    [Inject]
    public void Construct(GameStateManager gameStateManager, PlayerCamera playerCameraMove, InputUIDataLocator inputUIDataLocator)
    {
        _gameStateManager = gameStateManager;
        _playerCameraMove = playerCameraMove;
        _inputUIModel = new InputUIModel();
        _inputUIDataLocator = inputUIDataLocator;
        _inputUIView.Initialize();
        Bind();
    }

    private void Bind()
    {
        _gameStateManager.State.Prepend(_gameStateManager.State.CurrentValue).Subscribe(state =>
        {
            _inputUIModel.UpdateActive(state);
            _inputUIView.UpdateAngle(0);
            if (_gameStateManager.SubState.CurrentValue != SubGameState.Other) return;
            if (_gameStateManager.InputState.CurrentValue == GameInputState.Other)
            {
                var cmd = _inputUIDataLocator.Resolve(state);
                var movableData = cmd.GetInputUIMovableData();
                if (movableData != null) _inputUIView.UpdateMovable(movableData);
            }
        }).AddTo(this);

        _gameStateManager.InputState.Subscribe(state =>
          {
              if (_gameStateManager.SubState.CurrentValue != SubGameState.Other) return;
              if (state == GameInputState.Other)
              {
                  var uiDataCommand = _inputUIDataLocator.Resolve(_gameStateManager.State.CurrentValue);
                  var movableData = uiDataCommand.GetInputUIMovableData();
                  if (movableData != null)
                  {
                      _inputUIView.UpdateMovable(movableData);
                  }
                  return;
              }
              _inputUIView.UpdateMovable(new InputUIMovableData(false, false, false, false));
          }).AddTo(this);

        _gameStateManager.SubState.Subscribe(subState =>
        {
            if (subState == SubGameState.SelectJump)
            {
                var uiDataCommand = _inputUIDataLocator.Resolve(subState);
                var jumpMovableData = uiDataCommand.GetInputUIMovableData();
                _inputUIModel.UpdateActive(GameState.InGameInit);
                _inputUIView.UpdateMovable(jumpMovableData);
                return;
            }
            _inputUIModel.UpdateActive(_gameStateManager.State.CurrentValue);
            var cmd = _inputUIDataLocator.Resolve(_gameStateManager.State.CurrentValue);
            var movableData = cmd.GetInputUIMovableData();
            if (movableData != null) _inputUIView.UpdateMovable(movableData);
        }).AddTo(this);

        _inputUIModel.OnChangeActiveData.Subscribe(activeData =>
        {
            _inputUIView.UpdateActive(activeData);
        }).AddTo(this);

        _gameStateManager.OnInputUIRefresh.Subscribe(_ =>
        {
            if (_gameStateManager.InputState.CurrentValue != GameInputState.Other) return;
            if (_gameStateManager.SubState.CurrentValue != SubGameState.Other)
            {
                if (_gameStateManager.SubState.CurrentValue == SubGameState.SelectJump)
                {
                    var uiDataCommand = _inputUIDataLocator.Resolve(_gameStateManager.SubState.CurrentValue);
                    var jumpMovableData = uiDataCommand.GetInputUIMovableData();
                    _inputUIView.UpdateMovable(jumpMovableData);
                }
                return;
            }
            var cmd = _inputUIDataLocator.Resolve(_gameStateManager.State.CurrentValue);
            var movableData = cmd.GetInputUIMovableData();
            if (movableData != null) _inputUIView.UpdateMovable(movableData);
        }).AddTo(this);

        _playerCameraMove.CameraAngleY.Subscribe(angleY =>
        {
            if (_gameStateManager.State.CurrentValue != GameState.InGameIdle) return;
            _inputUIView.UpdateAngle(angleY);
        }).AddTo(this);
    }
}
