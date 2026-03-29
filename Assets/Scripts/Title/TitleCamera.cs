using UnityEngine;

public class TitleCamera : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    public void SetCameraActive(bool isActive)
    {
        _camera.gameObject.SetActive(isActive);
    }
}
