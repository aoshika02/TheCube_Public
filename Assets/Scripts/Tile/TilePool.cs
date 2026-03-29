using UnityEngine;
using VContainer;

public class TilePool : MonoBehaviour
{
    [SerializeField] private GameObject _tilePrefab;
    private GenericTilePool<TileObj> _tilePool;

    [Inject]
    public void Construct()
    {
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

    public void ReleaseAll()
    {
        _tilePool.ReleaseAll();
    }
}

public class GenericTilePool<T> : GenericObjectPool<T> where T : MonoBehaviour,IPool,ITileObj
{
    public GenericTilePool(GameObject instance, Transform parent) : base(instance, parent) { }

    public  void ReleaseAll(UseType useType)
    {
        foreach (var instance in _instances)
        {
            if (instance.IsGenericUse && instance.UseType == useType)
            {
                instance.UseType = UseType.None;
                Release(instance);
            }
        }
    }

}