using UnityEngine;
using VContainer;

public class GameInitialize : MonoBehaviour
{
    private GameStateManager _gameStateManager;

    [Inject]
    public void Construct(GameStateManager gameStateManager)
    {
        _gameStateManager = gameStateManager;
    }
    void Start()
    {
        _gameStateManager.ChangeState(GameState.TitleInit);
    }
}
