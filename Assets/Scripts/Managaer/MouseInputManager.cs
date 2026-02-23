using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem.LowLevel;
public class MouseInputManager : SingletonMonoBehaviour<MouseInputManager>
{
    [SerializeField] private PlayerAction _playerAction;
    [SerializeField] private Camera _mainCamera;
    public IObservable<GameObject> OnTapped => _onTapped;
    private readonly Subject<GameObject> _onTapped = new Subject<GameObject>();
    public IObservable<GameObject> OnCanceled => _onCanceled;
    private readonly Subject<GameObject> _onCanceled = new Subject<GameObject>();
    public IReadOnlyReactiveProperty<Vector3> MousePos => _mousePos;
    private readonly ReactiveProperty<Vector3> _mousePos = new ReactiveProperty<Vector3>();
    public IReadOnlyReactiveProperty<float> MouseClicked => _mouseClicked;
    private readonly ReactiveProperty<float> _mouseClicked = new ReactiveProperty<float>();

    private bool _isTapping = false;
    [SerializeField] private List<Canvas> _hitCanvases;
    private List<GraphicRaycaster> _raycasters = new List<GraphicRaycaster>();
    private PointerEventData _pointerEventData;
    private EventSystem _eventSystem;

    #region CallbackContext
    private void OnTapStartedCallback(InputAction.CallbackContext context)
    {
        OnTapStarted(context);
        _mouseClicked.Value = context.ReadValue<float>();
    }
    private void OnTapCanceledCallback(InputAction.CallbackContext context)
    {
        OnTapCanceled(context);
        _mouseClicked.Value = context.ReadValue<float>();
    }
    #endregion

    private void OnEnable()
    {
        _playerAction.Enable();

        _playerAction.PlayerTouch.Tap.started += OnTapStartedCallback;
        _playerAction.PlayerTouch.Tap.canceled += OnTapCanceledCallback;

       
        MouseReset(false);
    }

    private void OnDisable()
    {
        if (_playerAction != null)
        {
            _playerAction.PlayerTouch.Tap.started -= OnTapStartedCallback;
            _playerAction.PlayerTouch.Tap.canceled -= OnTapCanceledCallback;

            _playerAction.Disable();
        }
    }
    protected override void Awake()
    {
        if (!CheckInstance())
        {
            return;
        }

        if (_playerAction == null) _playerAction = new PlayerAction();

        foreach (var canvas in _hitCanvases)
        {
            _raycasters.Add(canvas.GetComponent<GraphicRaycaster>());
        }
        _eventSystem = EventSystem.current;
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
    /// <summary>
    /// タップ関連
    /// </summary>
    /// <param name="context"></param>
    private void OnTapStarted(InputAction.CallbackContext context)
    {
        TapEvent(_onTapped, true, context);
        _isTapping = true;
    }
    private void Update()
    {
        _mousePos.Value = Mouse.current.delta.ReadValue();
    }

    private void OnTapCanceled(InputAction.CallbackContext context)
    {
        TapEvent(_onCanceled, false, context);
        _isTapping = false;
    }
    private void TapEvent(Subject<GameObject> subject, bool isStarted, InputAction.CallbackContext context)
    {
        var tmpScreenPos = GetDeviceValue();

        if (tmpScreenPos == null) return;
        Vector3 screenPos = tmpScreenPos.Value;
        screenPos.z = _mainCamera.nearClipPlane;
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(screenPos);
        Vector3 direction = (worldPos - _mainCamera.transform.position).normalized;

        List<GameObject> uiHits = RaycastUI(screenPos);
        if (uiHits != null && uiHits.Count > 0)
        {
            subject.OnNext(uiHits[0]);
        }

        List<GameObject> hitObjs = Raycast3D(worldPos, direction);
        if (hitObjs != null)
        {
            _onTapped.OnNext(hitObjs[0]);
        }
    }
    /// <summary>
    /// デバイスに応じた座標を返す
    /// </summary>
    /// <returns></returns>
    private Vector3? GetDeviceValue()
    {
        //マウスが接続ならマウス座標を返す
        if (Mouse.current != null)
        {
            return Mouse.current.position.ReadValue();
        }
        //指の座標を返す
        if (Touchscreen.current != null)
        {
            if (Touchscreen.current.touches.Count == 1)
            {
                return Touchscreen.current.primaryTouch.position.ReadValue();
            }
        }
        //例外
        return null;
    }
    private List<GameObject> RaycastUI(Vector2 screenPosition)
    {
        _pointerEventData = new PointerEventData(_eventSystem)
        {
            position = screenPosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        List<List<RaycastResult>> resultDatas = new List<List<RaycastResult>>();
        foreach (var cast in _raycasters)
        {
            cast.Raycast(_pointerEventData, results);
            resultDatas.Add(new List<RaycastResult>(results));
            results.Clear();
        }

        return resultDatas.SelectMany(list => list).Select(r => r.gameObject).ToList();
    }
    private List<GameObject> Raycast3D(
        Vector3 origin,
        Vector3 direction,
        float distance = Mathf.Infinity)
    {
        RaycastHit[] hitObjs = Physics.RaycastAll(origin, direction, distance);
        //Debug.DrawRay(origin, direction, Color.red, 1f);
        if (hitObjs.Length == 0) return null;
        RaycastHit[] sortedHits = hitObjs.OrderBy(hit => hit.distance).ToArray();
        List<GameObject> hisObjs = new List<GameObject>();
        foreach (var hitObj in sortedHits)
        {
            if (hitObj.collider == null) continue;
            hisObjs.Add(hitObj.collider.gameObject);
        }
        return hisObjs;
    }
}
