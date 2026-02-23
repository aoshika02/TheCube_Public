using UnityEngine;

public class InfoViewPool : SingletonMonoBehaviour<InfoViewPool>
{
    [SerializeField] private GameObject _infoViewPrefab;
    private GenericObjectPool<InfoView> _infoViewPool;

    protected override void Awake()
    {
        if (!CheckInstance()) return;
        _infoViewPool = new GenericObjectPool<InfoView>(_infoViewPrefab, transform);
    }

    public InfoView GetInfoView(Transform parent)
    {
        var _infoView = _infoViewPool.Get();
        _infoView.transform.SetParent(parent, false);
        return _infoView;
    }

    public void ReleaseInfoView(InfoView infoView)
    {
        _infoViewPool.Release(infoView);
    }
}
