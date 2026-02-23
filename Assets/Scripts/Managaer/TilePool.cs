using UnityEngine;
public class TilePool : SingletonMonoBehaviour<TilePool>
{
    [SerializeField] private GameObject _tilePrefab;
    private GenericTilePool<TileObj> _tilePool;
    protected override void Awake()
    {
        if (CheckInstance() == false) return;
        _tilePool = new GenericTilePool<TileObj>(_tilePrefab, transform);
    }

    public TileObj GetTile()
    {
        return _tilePool.Get();
    }

    public void ReleaseTile(TileObj tile)
    {
        _tilePool.Release(tile);
    }

    public void ReleaseAll(UseType useType) 
    { 
        _tilePool.ReleaseAll(useType);
    }
}

public class GenericTilePool<T> : GenericObjectPool<T> where T : MonoBehaviour,IPool,ITileObj
{
    public GenericTilePool(GameObject instance, Transform parent) : base(instance, parent) { }

    public  void ReleaseAll(UseType useType)
    {
        foreach (var instance in _instances)
        {
            if (instance.UseType == useType)
            {
                instance.UseType = UseType.None;
                Release(instance);
            }
        }
    }
}