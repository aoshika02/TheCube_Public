using UnityEngine;

public class InGameInputUICommand : IInputUIDataCommand
{
    private PlayerCube _playerCube;
    private TileManager _tileManager;
    private PlayerMoveProcessor _playerMoveProcessor;

    public InGameInputUICommand(
        PlayerCube playerCube, 
        TileManager tileManager,
        PlayerMoveProcessor playerMoveProcessor)
    {
        _playerCube = playerCube;
        _tileManager = tileManager;
        _playerMoveProcessor = playerMoveProcessor;
    }

    public InputUIMovableData GetInputUIMovableData()
    {
        return new InputUIMovableData(
                IsMovable(Vector2Int.up) ,
                IsMovable(Vector2Int.left),
                IsMovable(Vector2Int.down),
                IsMovable(Vector2Int.right)
            );
    }

    private bool IsMovable(Vector2Int moveDir)
    {
        return IsMovableAsTile(moveDir) && IsMovableAsPlayer(moveDir);
    }

    private bool IsMovableAsTile(Vector2Int moveDir)
    {
        var cubeTransform = _playerCube.GetParent(ParentType.Rotate);
        var movedPos = CubeMoveUtil.GetNextPos(cubeTransform, moveDir);
        var targetTileType = _tileManager.GetTileType(movedPos);
        return targetTileType != TileType.None;
    }

    private bool IsMovableAsPlayer(Vector2Int moveDir)
    {
        var cubeTransform = _playerCube.GetParent(ParentType.Rotate);
        var movedPos = CubeMoveUtil.GetNextPos(cubeTransform, moveDir);
        var targetType = _tileManager.GetDetachmentType(movedPos);
        return _playerMoveProcessor.MoveableCheck(moveDir, targetType) != MoveType.UnMove;
    }
}
