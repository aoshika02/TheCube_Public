using UnityEngine;
using UnityEngine.InputSystem;
using R3;
using UnityEngine.InputSystem.LowLevel;
using VContainer;
public class MouseInputManager :MonoBehaviour
{
    public ReadOnlyReactiveProperty<Vector3> MousePos => _mousePos;
    private readonly ReactiveProperty<Vector3> _mousePos = new ReactiveProperty<Vector3>();

    [Inject]
    public void Construct()
    {
        MouseReset();
    }

    public void MouseReset(bool isView = false)
    {
        // 画面中央座標を計算
        Vector2 center = new Vector2(Screen.width / 2f, Screen.height / 2f);

        // 実際のカーソル位置を中央に移動
        Mouse.current.WarpCursorPosition(center);

        // InputSystemにも反映（同期用）
        InputState.Change(Mouse.current.position, center);
        if (isView)
        {
            // 中央に固定
            Cursor.lockState = CursorLockMode.None;
            return;
        }
        Cursor.lockState = CursorLockMode.Locked;
    }
   
    private void Update()
    {
        _mousePos.Value = Mouse.current.delta.ReadValue();
    }
}
