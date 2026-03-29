using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using TileAnimUtil;

public class TileObj : MonoBehaviour, IPool, ITileObj
{
    public bool IsGenericUse { get; set; }
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] protected GameObject _tileObj;
    private Material _material;

    private CancellationTokenSource _tokenSource = new CancellationTokenSource();

    public TileType TileType => _tileType;
    [field: SerializeField]
    public UseType UseType { get; set; }

    [SerializeField] private TileType _tileType;

    public void Init(TileDataSet tileInitData, Texture2D frameTexture, TileType tileType, UseType useType, ITileAnim tileAnim = null, bool animStart = true, string layerName = "Default")
    {
        _tileType = tileType;
        UseType = useType;

        var layer = LayerMask.NameToLayer(layerName);
        foreach (var t in GetComponentsInChildren<Transform>(true))
        {
            t.gameObject.layer = layer;
        }

        if (_material == null)
        {
            if (_meshRenderer.materials == null)
            {
                Debug.LogError($"{nameof(Material)}がありません");
                return;
            }
            if (_meshRenderer.materials.Length <= 1)
            {
                Debug.LogError($"{nameof(Material)}が指定数分ありません {_meshRenderer.materials.Length}");
                return;
            }
            _material = _meshRenderer.materials[1];
        }

        _material.MaterialInit(
            frameTexture,
            tileInitData.IconTexture,
            tileInitData.BaseColor,
            tileInitData.FrameColor,
            tileInitData.IconColor,
            !(tileType == TileType.Normal || tileInitData.IconTexture == null));

        if (!animStart || tileAnim == null) return;
        tileAnim.AnimFlow(_material, _tokenSource).Forget();
    }

    public void OnRelease()
    {
        _tokenSource.Cancel();
        _tokenSource.Dispose();
        _tokenSource = new CancellationTokenSource();
        gameObject.SetActive(false);
    }

    public void OnReuse()
    {
        gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        _tokenSource.Cancel();
        _tokenSource.Dispose();
    }
}
