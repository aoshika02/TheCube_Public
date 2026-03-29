
using System.Collections.Generic;

public class InputUIDataLocator 
{
    private readonly Dictionary<GameState, IInputUIDataCommand> _commandDict = new Dictionary<GameState, IInputUIDataCommand>();
    private readonly IInputUIDataCommand _defaultCommand;

    public InputUIDataLocator(IInputUIDataCommand defaultCommand)
    {
        _defaultCommand = defaultCommand;
    }

    public void Register(GameState state, IInputUIDataCommand command)
    {
        _commandDict[state] = command;
    }

    public IInputUIDataCommand Resolve(GameState state)
    {
        return _commandDict.TryGetValue(state, out var command) ? command : _defaultCommand;
    }
}