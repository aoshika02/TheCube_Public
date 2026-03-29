using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using VContainer;

public class DialogPresenter : MonoBehaviour
{
    private DialogModel _dialogModel;
    [SerializeField] private DialogView _dialogView;
    private TextDataset _textDataset;
    private InputManager _inputManager;
    private GameStateManager _gameStateManager;
    private bool _isInput = false;

    [Inject]
    public void Construct(
        TextDataset textDataset,
        InputManager inputManager,
        GameStateManager gameStateManager,
        DialogModel dialogModel)
    {
        _textDataset = textDataset;
        _inputManager = inputManager;
        _gameStateManager = gameStateManager;
        _dialogModel = dialogModel;
        _dialogView.Initialized();
        Bind();
    }

    private void Bind()
    {
        _inputManager.Enter.Subscribe(x =>
        {
            if (x != 1) return;
            if (_gameStateManager.InputState.CurrentValue != GameInputState.Dialog) return;
            _isInput = true;

        }).AddTo(this);

        _dialogModel.OnAddDialog.Subscribe(async _ =>
        {
            if (_gameStateManager.InputState.CurrentValue == GameInputState.Dialog) return;
            _gameStateManager.SetInputState(GameInputState.Dialog);
            await _dialogView.ShowDialog(Vector3.one, token: destroyCancellationToken);
            while (_dialogModel.HasDialog())
            {
                if (_dialogModel.TryDequeue(out var dialogEventType))
                {
                    if (dialogEventType.HasValue)
                    {
                        _dialogView.SetText(_textDataset.GetText(dialogEventType.Value));
                        _isInput = false;
                        await UniTask.WaitUntil(() => _isInput, cancellationToken: destroyCancellationToken);
                    }
                }
            }
            await _dialogView.ShowDialog(Vector3.zero, token: destroyCancellationToken);
            _dialogView.SetText(string.Empty);
            _gameStateManager.SetInputState(GameInputState.Other);
        }).AddTo(this);
    }
}

public enum DialogEventType
{
    None = 0,
    DeleteSaveData = 1,
    Normal = 2,
    Goal = 3,
    Skip = 4,
    Reset = 5,
    Warp = 6,
    Arrow = 7,
    Jump = 8,
    Tutorial01 = 51,
    Tutorial02 = 52,
    Tutorial03 = 53,
    Tutorial04 = 54,
}