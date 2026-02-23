using Cysharp.Threading.Tasks;

public class TutorialManager : SingletonMonoBehaviour<TutorialManager>
{
    private DialogManager _dialogManager;
    private GameInputStateManager _gameInputStateManager;
    public bool IsFinished => _isFinished;
    private bool _isFinished = false;

    protected override void Awake()
    {
        if (CheckInstance() == false) return;
        _dialogManager = DialogManager.Instance;
        _gameInputStateManager = GameInputStateManager.Instance;
    }

    public void Init(bool isFinished)
    {
        _isFinished = isFinished;
    }

    public async UniTask TutorialFlow()
    {
        if (_isFinished) return;
        _dialogManager.AddDialog(DialogEventType.Tutorial01);
        _dialogManager.AddDialog(DialogEventType.Tutorial02);
        _dialogManager.AddDialog(DialogEventType.Tutorial03);
        _dialogManager.AddDialog(DialogEventType.Tutorial04);

        await UniTask.WaitUntil(() =>
        _gameInputStateManager.CurrentInputState == GameInputState.Other);
        _isFinished = true;
    }

}
