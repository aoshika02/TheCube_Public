using System.Collections.Generic;

public class MoveCommandLocator 
{
    private readonly Dictionary<TileType, IMoveCommand> _commandDict = new Dictionary<TileType, IMoveCommand>();
    private readonly IMoveCommand _defaultCommand;

    public MoveCommandLocator(IMoveCommand defaultCommand)
    {
        _defaultCommand = defaultCommand;
    }

    public void Register(TileType tileType, IMoveCommand command)
    {
        _commandDict[tileType] = command;
    }

    public IMoveCommand Resolve(TileType tileType)
    {
        return _commandDict.TryGetValue(tileType, out var command) ? command : _defaultCommand;
    }
}
