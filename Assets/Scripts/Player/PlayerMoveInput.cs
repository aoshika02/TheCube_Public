using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;
using System;
using Extension;

public class PlayerMoveInput : SingletonMonoBehaviour<PlayerMoveInput>
{
    private Vector2Int _moveInput;
    public IObservable<Vector2Int> OnMoveInput => _onMoveInput;
    private readonly Subject<Vector2Int> _onMoveInput = new Subject<Vector2Int>();

    protected override void Awake()
    {
        if (!CheckInstance()) return;
    }
    private void Start()
    {
        InputManager.Instance.KeyW.Subscribe(_ =>
        {
            if (_ != 1) return;
            OnInput(ArrowType.Up);
        }).AddTo(this);

        InputManager.Instance.KeyS.Subscribe(_ =>
        {
            if (_ != 1) return;
            OnInput(ArrowType.Down);
        }).AddTo(this);

        InputManager.Instance.KeyA.Subscribe(_ =>
        {
            if (_ != 1) return;
            OnInput(ArrowType.Left);
        }).AddTo(this);

        InputManager.Instance.KeyD.Subscribe(_ =>
        {
            if (_ != 1) return;
            OnInput(ArrowType.Right);
        }).AddTo(this);
    }
    private void OnInput(ArrowType arrowType)
    {
        _moveInput = arrowType.Arrow2Vector2Int();
        if (_moveInput != Vector2Int.zero)
        {
            _onMoveInput.OnNext(_moveInput);
            _moveInput = Vector2Int.zero;
        }
    }
}
