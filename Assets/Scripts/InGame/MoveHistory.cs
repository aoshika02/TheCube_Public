using System.Collections.Generic;

public class MoveHistory
{
    private readonly List<MoveInfo> _infos = new();
    private int _sessionStart = 0;

    public List<MoveInfo> CurrentSessionInfos =>
        _infos.GetRange(_sessionStart, _infos.Count - _sessionStart);

    public int Count => _infos.Count - _sessionStart;

    public void Add(MoveInfo info) => _infos.Add(info);

    public void RemoveLast()
    {
        if (_infos.Count > _sessionStart)
            _infos.RemoveAt(_infos.Count - 1);
    }

    public MoveInfo GetFromEnd(int offset)
    {
        int index = _infos.Count - 1 - offset;
        return index >= _sessionStart ? _infos[index] : null;
    }

    public void StartNewSession() => _sessionStart = _infos.Count;

    public void Clear()
    {
        _infos.Clear();
        _sessionStart = 0;
    }
}
