using R3;
public class GameStateManager
{
    public ReadOnlyReactiveProperty<GameState> State => _gameState;
    private ReactiveProperty<GameState> _gameState = new ReactiveProperty<GameState>();

    public ReadOnlyReactiveProperty<GameInputState> InputState => _gameInputState;
    private ReactiveProperty<GameInputState> _gameInputState = new ReactiveProperty<GameInputState>();

    public ReadOnlyReactiveProperty<SubGameState> SubState => _subGameState;
    private ReactiveProperty<SubGameState> _subGameState = new ReactiveProperty<SubGameState>();

    public Observable<Unit> OnInputUIRefresh => _onInputUIRefresh;
    private Subject<Unit> _onInputUIRefresh = new Subject<Unit>();

    public GameStateManager()
    {
        _gameState = new ReactiveProperty<GameState>(GameState.None);
        _gameInputState = new ReactiveProperty<GameInputState>(GameInputState.None);
        _subGameState = new ReactiveProperty<SubGameState>(SubGameState.None);
    }

    public void ChangeState(GameState newState)
    {
        _gameState.Value = newState;
    }

    public void ChangeInputState(GameInputState newInputState)
    {
        _gameInputState.Value = newInputState;
    }

    public void ChangeSubState(SubGameState newSubState)
    {
        _subGameState.Value = newSubState;
    }

    public void RequestInputUIRefresh()
    {
        _onInputUIRefresh.OnNext(Unit.Default);
    }
}
public enum GameState
{
    None,
    TitleInit,
    TitleIdle,
    TitleShutdown,
    StageSelectInit,
    StageSelectIdle,
    StageSelectShutdown,
    InGameInit,
    InGameIdle,
    InGameShutdown,
    InGameReset,
}

public enum GameInputState
{
    None,
    Other,
    Dialog,
    Moving,
}

public enum SubGameState
{
    None,
    Other,
    SelectJump,
}

