using UnityEngine;

public class CubeTile : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Vector3 _startPos;
    [SerializeField] private Quaternion _startRot;
    private Material _material;
    private int _emissionColorId = Shader.PropertyToID(ParamConsts.EMISSION_COLOR);

    public void Init()
    {
        _startPos = transform.position;
        _startRot = transform.rotation;
    }

    public void SetColor(Color color)
    {
        if(MaterialCheck() == false) return;
        _material.SetColor(_emissionColorId, color);
    }

    public void ResetTransform()
    {
        transform.position = _startPos;
        transform.rotation = _startRot;
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
