using UnityEngine;

public class SelectCameraManager : MonoBehaviour
{
    private bool _isFollow = true;
    private Transform _cubeTransform;
    [SerializeField] private Camera _camera;

    public void Init(PlayerCube playerCube,int posX)
    {
        SetIsFollow(false);
        SetCameraPos(posX);
        _cubeTransform = playerCube.GetParent(ParentType.Rotate);
    }

    public void SetCameraActive(bool isActive)
    {
        _camera.gameObject.SetActive(isActive);
    }

    public void SetIsFollow(bool isFollow)
    {
        _isFollow = isFollow;
    }

    public void LateUpdate()
    {
        if (_cubeTransform == null) return;
        if (!_isFollow) return;
        SetCameraPos(_cubeTransform.position.x);
    }

    public void SetCameraPos(float posX)
    {
        transform.position = new Vector3(
            posX,
            transform.position.y,
            transform.position.z);
    }
}
