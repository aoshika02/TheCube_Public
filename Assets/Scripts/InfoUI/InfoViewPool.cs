using UnityEngine;
using VContainer;

public class InfoViewPool : MonoBehaviour
{
    [SerializeField] private GameObject _infoViewPrefab;
    private GenericObjectPool<InfoView> _infoViewPool;

    [Inject]
    public void Construct()
    {
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
