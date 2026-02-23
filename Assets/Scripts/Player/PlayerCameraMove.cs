using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
public class PlayerCameraMove : SingletonMonoBehaviour<PlayerCameraMove>
{
    [SerializeField] private Transform _cameraTransform;
    private bool _isRotate;
    //回転系統
    [SerializeField]
    float _maxCameraRotateY;

    [SerializeField]
    float _minCameraRotateY;

    private bool _isFollow = true;
    private float _cameraAngleX = 0;
    [SerializeField] private float _rotationSpeed = 1f;

    [SerializeField] private float _sensitivityY = 0.5f;
    [SerializeField] private float _sensitivityX = 1.5f;

    private PlayerCube _cube;

    public IReadOnlyReactiveProperty<float> CameraAngleY => _cameraAngleY;
    private readonly ReactiveProperty<float> _cameraAngleY = new ReactiveProperty<float>(0);
    void Start()
    {
        _isRotate = true;
        _cube = PlayerCube.Instance;
        _cameraAngleX = _maxCameraRotateY;
        SetRotateX(_cameraAngleX);
        //playerの移動イベントの登録
        MouseInputManager.Instance.MousePos.Subscribe(x =>
        {
            if (_isRotate == false) return;
            CameraMove(x);
        }).AddTo(this);
    }

    public void SetRotateEnable(bool isEnable)
    {
        _isRotate = isEnable;
    }

    public void SetFollow(bool isFollow)
    {
        _isFollow = isFollow;
    }
    /// <summary>
    /// カメラの回転処理
    /// </summary>
    /// <param name="mousePos"></param>
    private void CameraMove(Vector2 mousePos)
    {
        float yaw = mousePos.x * _sensitivityY;
        _cameraTransform.transform.Rotate(0, yaw * _rotationSpeed, 0);
        _cameraAngleY.Value = _cameraTransform.eulerAngles.y;

        _cameraAngleX -= mousePos.y * _rotationSpeed * _sensitivityX;
        _cameraAngleX = Mathf.Clamp(_cameraAngleX, _minCameraRotateY, _maxCameraRotateY);

        SetRotateX(_cameraAngleX);
    }

    public void SetRotateX(float cameraAngleX)
    {
        var sampleAngle = _cameraTransform.eulerAngles;
        sampleAngle.x = cameraAngleX;
        sampleAngle.z = 0;
        _cameraTransform.eulerAngles = sampleAngle;
    }

    public void SetPos(Vector3 pos)
    {
        _cameraTransform.position = pos;
    }

    private void LateUpdate()
    {
        if (_isFollow == false) return;
        var followPos = _cube.GetPlayerPos();
        followPos.y = 0;
        _cameraTransform.position = followPos;
    }
}