using UnityEngine;

public class SelectCameraManager : SingletonMonoBehaviour<SelectCameraManager>
{
    private bool _isFollow = true;
    private PlayerCube _cube;

    public void Init(PlayerCube playerCube,int posX)
    {
        SetIsFollow(false);
        SetCameraPos(posX);
        _cube = playerCube;
    }

    public void SetIsFollow(bool isFollow)
    {
        _isFollow = isFollow;
    }

    public void LateUpdate()
    {
        if (_cube == null) return;
        if (!_isFollow) return;
        SetCameraPos(_cube.GetPlayerPos().x);
    }

    public void SetCameraPos(float posX)
    {
        transform.position = new Vector3(
            posX,
            transform.position.y,
            transform.position.z);
    }
}
