using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SceneLoadManager : SingletonMonoBehaviour<SceneLoadManager>
{
    protected override void Awake()
    {
        if (CheckInstance() == false)
        {
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public async UniTask LoadSceneAsync(SceneType sceneType)
    {
        await SceneManager.LoadSceneAsync((int)sceneType);
    }

    public async UniTask LoadSceneAsync(int stageIndex)
    {
        await LoadSceneAsync(SceneType.Main);
        await GameManager.Instance.Init(stageIndex);
    }

    public async UniTask LoadStageSelectAsync(int stageIndex = 0, bool isBackFromInGame = false)
    {
        await LoadSceneAsync(SceneType.StageSelect);
        await StageSelectManager.Instance.Init(isBackFromInGame, stageIndex);
    }
}

public enum SceneType
{
    None = -1,
    Title = 0,
    StageSelect = 1,
    Main = 2,
}
