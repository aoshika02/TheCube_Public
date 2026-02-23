using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using TileAnimUtil;

public class TileObj : MonoBehaviour, IPool, ITileObj
{
    public bool IsGenericUse { get; set; }
    [SerializeField] private int _id;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] protected GameObject _tileModel;
    private Material _material;

    private CancellationTokenSource _tokenSource = new CancellationTokenSource();

    public TileType TileType => _tileType;
    [field: SerializeField]
    public UseType UseType { get; set; }

    [SerializeField] private TileType _tileType;

    public void Init(TileDataSet tileInitData, Texture2D frameTexture, TileType tileType, UseType useType, int id = 0, bool animStart = true, string layerName = "Default")
    {
        _id = id;
        _tileType = tileType;
        UseType = useType;

        var layer = LayerMask.NameToLayer(layerName);
        gameObject.layer = layer;
        _tileModel.layer = layer;

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

        if (!animStart) return;
        ShaderAnim(_tileType).Forget();
    }

    private async UniTask ShaderAnim(TileType tileType)
    {
        switch (tileType)
        {
            case TileType.Reset:
                await _material.RotateFlow(_tokenSource);
                break;

            case TileType.Warp:
                await _material.ReverseScaleFlow(_tokenSource);
                break;

            case TileType.UpperArrow:
                await _material.ArrowFlow(ArrowType.Up, _tokenSource);
                break;

            case TileType.LeftArrow:
                await _material.ArrowFlow(ArrowType.Left, _tokenSource);
                break;

            case TileType.DownArrow:
                await _material.ArrowFlow(ArrowType.Down, _tokenSource);
                break;

            case TileType.RightArrow:
                await _material.ArrowFlow(ArrowType.Right, _tokenSource);
                break;

            case TileType.Goal:
                await _material.ScaleFlow(_tokenSource);
                break;

            case TileType.JumpUp:
            case TileType.JumpDown:
            case TileType.JumpLeft:
            case TileType.JumpRight:
                await _material.BounceFlow(_tokenSource);
                break;
            default:
                Debug.LogWarning($"TileType {tileType} はアニメーションが設定されていません");
                break;
        }
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
