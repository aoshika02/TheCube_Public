public interface IPool
{
    public bool IsGenericUse { get; set; }
    public void OnReuse();
    void OnRelease();
}
