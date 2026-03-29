using UnityEngine;

public class CubeFace : MonoBehaviour
{
    [SerializeField] private DirectionType _directionType;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Vector3 _startPos;
    [SerializeField] private Quaternion _startRot;
    public DirectionType DirectionType => _directionType;
    private Material _material;
    private int _emissionColorId = Shader.PropertyToID(ParamConsts.EMISSION_COLOR);

    public void Init()
    {
        _startPos = transform.localPosition;
        _startRot = transform.localRotation;
    }

    public void SetColor(Color color)
    {
        if(MaterialCheck() == false) return;
        _material.SetColor(_emissionColorId, color);
    }

    public void ResetTransform()
    {
        transform.localPosition = _startPos;
        transform.localRotation = _startRot;
    }

    private bool MaterialCheck()
    {
        if (_material == null)
        {
            _material = _meshRenderer.material;
        }
        return _material != null;
    }
}
