using Cysharp.Threading.Tasks;

public class TutorialManager
{
    private readonly GameStateManager _gameStateManager;
    private readonly DialogModel _dialogModel;
    public bool IsFinished => _isFinished;
    private bool _isFinished = false;

    public TutorialManager(GameStateManager gameStateManager, DialogModel dialogModel, LoadManager loadManager)
    {
        _gameStateManager = gameStateManager;
        _dialogModel = dialogModel;
        _isFinished = loadManager.IsTutorialFinished();
    }

    public async UniTask TutorialFlow()
    {
        if (_isFinished) return;
        _dialogModel.AddDialog(DialogEventType.Tutorial01);
        _dialogModel.AddDialog(DialogEventType.Tutorial02);
        _dialogModel.AddDialog(DialogEventType.Tutorial03);
        _dialogModel.AddDialog(DialogEventType.Tutorial04);

        await UniTask.WaitUntil(() =>
        _gameStateManager.InputState.CurrentValue == GameInputState.Other);
        _isFinished = true;
    }

    public void SetTutorialFinished(bool isFinish)
    {
        _isFinished = isFinish;
    }
}
