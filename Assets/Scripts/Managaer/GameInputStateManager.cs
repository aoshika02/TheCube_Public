using System;
using UniRx;

public class GameInputStateManager : SingletonMonoBehaviour<GameInputStateManager>
{
    public GameInputState CurrentInputState { get; private set; } = GameInputState.None;
    public IObservable<GameInputState> OnInputStateChanged => _onInputStateChanged;
    private Subject<GameInputState> _onInputStateChanged = new Subject<GameInputState>();

    protected override void Awake()
    {
        if(!CheckInstance()) 
        {
            Destroy(gameObject);
            return; 
        }
        DontDestroyOnLoad(gameObject);
    }

    public void SetInputState(GameInputState newState)
    {
        if (CurrentInputState != newState)
        {
            CurrentInputState = newState;
            _onInputStateChanged.OnNext(newState);
        }
    }
}

public enum GameInputState
{
    None = 0,
    Dialog = 1,
    Other = 2,
    Moving = 3,
}
