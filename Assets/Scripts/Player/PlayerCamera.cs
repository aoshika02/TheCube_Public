using UnityEngine;
using R3;
using Cysharp.Threading.Tasks;
using VContainer;
public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Camera _camera;
    private bool _isRotate = false;
    //回転系統
    [SerializeField]
    float _maxCameraRotateY;

    [SerializeField]
    float _minCameraRotateY;

    private bool _isFollow = false;
    private float _cameraAngleX = 0;
    [SerializeField] private float _rotationSpeed = 1f;

    [SerializeField] private float _sensitivityY = 0.5f;
    [SerializeField] private float _sensitivityX = 1.5f;

    private MouseInputManager _mouseInputManager;
    private GameStateManager _gameStateManager;
    private PlayerCube _cube;
    private Transform _cubeTransform;

    public ReadOnlyReactiveProperty<float> CameraAngleY => _cameraAngleY;
    private readonly ReactiveProperty<float> _cameraAngleY = new ReactiveProperty<float>(0);
    [Inject]
    public void Construct(MouseInputManager mouseInputManager, PlayerCube playerCube, GameStateManager gameStateManager)
    {
        _mouseInputManager = mouseInputManager;
        _cube = playerCube;
        _gameStateManager = gameStateManager;
        _isRotate = false;
        _isFollow = false;
        Initialized();
        Bind();
    }

    private void Initialized()
    {
        _cubeTransform = _cube.GetParent(ParentType.Rotate);
        _cameraAngleX = _maxCameraRotateY;
        SetRotateX(_cameraAngleX);
        _camera.gameObject.SetActive(false);
    }

    private void Bind()
    {
        _gameStateManager.State.Subscribe(state =>
        {
            if (state == GameState.InGameInit)
            {
                _cameraAngleX = _maxCameraRotateY;
                SetRotateX(_cameraAngleX);
                _isRotate = true;
                _isFollow = true;
                _camera.gameObject.SetActive(true);
            }
            else if (state == GameState.InGameShutdown)
            {
                _isRotate = false;
                _isFollow = false;
            }
        }).AddTo(this);

        _mouseInputManager.MousePos.Subscribe(x =>
        {
            if (_isRotate == false) return;
            CameraMove(x);
        }).AddTo(this);
    }

    public void SetFollow(bool isFollow)
    {
        _isFollow = isFollow;
    }

    private void CameraMove(Vector2 mousePos)
    {
        float yaw = mousePos.x * _sensitivityY;
        _cameraTransform.transform.Rotate(0, yaw * _rotationSpeed, 0);
        _cameraAngleY.Value = _cameraTransform.eulerAngles.y;

        _cameraAngleX -= mousePos.y * _rotationSpeed * _sensitivityX;
        _cameraAngleX = Mathf.Clamp(_cameraAngleX, _minCameraRotateY, _maxCameraRotateY);

        SetRotateX(_cameraAngleX);
    }

    private void SetRotateX(float cameraAngleX)
    {
        var sampleAngle = _cameraTransform.eulerAngles;
        sampleAngle.x = cameraAngleX;
        sampleAngle.z = 0;
        _cameraTransform.eulerAngles = sampleAngle;
    }

    public void SetCameraActive(bool isActive)
    {
        _camera.gameObject.SetActive(isActive);
    }

    public void SetPos(Vector3 pos)
    {
        _cameraTransform.position = pos;
    }

    private void LateUpdate()
    {
        if (_isFollow == false) return;
        var followPos = _cubeTransform.position;
        followPos.y = 0;
        _cameraTransform.position = followPos;
    }
}