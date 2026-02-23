using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

public class TitleManager : SingletonMonoBehaviour<TitleManager>
{
    private bool _isLoading = false;
    private GameInputStateManager _gameInputStateManager;
    private void Start()
    {
        _isLoading = false;
        _gameInputStateManager = GameInputStateManager.Instance;
        _gameInputStateManager.SetInputState(GameInputState.None);

        InputManager.Instance.Space.Subscribe(x =>
        {
            if (x != 1) return;
            if(_gameInputStateManager.CurrentInputState != GameInputState.Other) return;
            LoadStageSelect().Forget();
        }).AddTo(this);

        InputManager.Instance.KeyX.Subscribe(x =>
        {
            if (x != 1) return;
            if (SaveLoadManager.Instance.DeleteSaveData())
            {
                DialogManager.Instance.AddDialog(DialogEventType.DeleteSaveData);
            }
        }).AddTo(this);

        _gameInputStateManager.SetInputState(GameInputState.Other);
    }

    private async UniTask LoadStageSelect()
    {
        if (_isLoading) return;
        _isLoading = true;
        _gameInputStateManager.SetInputState(GameInputState.None);
        await FadeManager.Instance.FadeOut();
        var saveData = SaveLoadManager.Instance.GetGameSaveData();

        if(saveData == null)
        {
            Debug.LogWarning("セーブデータの取得に失敗しました。ステージセレクトへ移動します。");
            await SceneLoadManager.Instance.LoadStageSelectAsync();
            return;
        }
        Debug.Log($"タイトル画面からステージセレクトへ移動します。最終選択ステージ: {saveData.LastStageID}");
        await SceneLoadManager.Instance.LoadStageSelectAsync(saveData.LastStageID);
    }
}
