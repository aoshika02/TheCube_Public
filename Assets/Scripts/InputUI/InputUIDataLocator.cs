
using System.Collections.Generic;

public class InputUIDataLocator 
{
    private readonly Dictionary<GameState, IInputUIDataCommand> _commandDict = new Dictionary<GameState, IInputUIDataCommand>();
    private readonly Dictionary<SubGameState, IInputUIDataCommand> _subStateCommandDict = new Dictionary<SubGameState, IInputUIDataCommand>();
    private readonly IInputUIDataCommand _defaultCommand;

    public InputUIDataLocator(IInputUIDataCommand defaultCommand)
    {
        _defaultCommand = defaultCommand;
    }

    public void Register(GameState state, IInputUIDataCommand command)
    {
        _commandDict[state] = command;
    }

    public void Register(SubGameState subState, IInputUIDataCommand command)
    {
        _subStateCommandDict[subState] = command;
    }

    public IInputUIDataCommand Resolve(GameState state)
    {
        return _commandDict.TryGetValue(state, out var command) ? command : _defaultCommand;
    }

    public IInputUIDataCommand Resolve(SubGameState subState)
    {
        return _subStateCommandDict.TryGetValue(subState, out var command) ? command : _defaultCommand;
    }
}