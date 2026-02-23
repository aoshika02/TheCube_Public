using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

public class DialogManager : SingletonMonoBehaviour<DialogManager>
{
    [SerializeField] private TextMeshProUGUI _dialogText;
    [SerializeField] private RectTransform _dialogRoot;
    private TextDataset _textDataset;
    private GameInputStateManager _gameInputStateManager;
    private bool _isShowing = false;
    private bool _isCall = false;
    private bool _isInput = false;
    private readonly Queue<DialogEventType> _dialogEventTypeQueue = new Queue<DialogEventType>();

    protected override void Awake()
    {
        if (CheckInstance() == false)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        _textDataset = TextDataset.Instance;
        _dialogRoot.localScale = Vector3.zero;
    }

    private void Start()
    {
        _gameInputStateManager = GameInputStateManager.Instance;
        InputManager.Instance.Enter.Subscribe(x =>
        {
            if (x != 1) return;
            if (_gameInputStateManager.CurrentInputState != GameInputState.Dialog) return;
            if (_isShowing == false) return;
            _isInput = true;
        }).AddTo(this);
    }

    public void AddDialog(DialogEventType type)
    {
        _dialogEventTypeQueue.Enqueue(type);
        ShowDialog().Forget();
    }

    private async UniTask ShowDialog(float duration = 0.25f)
    {
        if (_isCall) return;
        _isCall = true;

        _gameInputStateManager.SetInputState(GameInputState.Dialog);
        await _dialogRoot.DOScale(Vector3.one, duration).ToUniTask();
        _isShowing = true;
        while (_dialogEventTypeQueue.Count > 0)
        {
            SetText(_dialogEventTypeQueue.Dequeue());

            _isInput = false;
            await UniTask.WaitUntil(() => _isInput);
        }

        _isShowing = false;
        await _dialogRoot.DOScale(Vector3.zero, duration).ToUniTask();
        _gameInputStateManager.SetInputState(GameInputState.Other);
        _dialogText.text = "";
        _isCall = false;
    }

    private void SetText(DialogEventType dialogEventType)
    {
        string message = _textDataset.GetText(dialogEventType);
        _dialogText.text = message;
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