using UnityEngine;
using UnityEngine.UI;

public class ArrowObj : MonoBehaviour
{
    [SerializeField] private RectTransform _rabelText;
    [SerializeField] private float _defaultAngle = 0f;
    [SerializeField] private Image _arrowCoverImage;

    public void SetAngle(float angle)
    {
        _rabelText.localEulerAngles = new Vector3(0f, 0f, angle + _defaultAngle);
    }

    public void SetMovable(bool isMovable)
    {
        _arrowCoverImage.gameObject.SetActive(!isMovable);
    }
}
