using R3;
using System.Collections.Generic;

public class DialogModel 
{
    public Observable<Unit> OnAddDialog => _onAddDialog;
    private Subject<Unit> _onAddDialog = new Subject<Unit>();
    private readonly Queue<DialogEventType> _dialogEventTypeQueue = new Queue<DialogEventType>();

    public void AddDialog(DialogEventType type)
    {
        _dialogEventTypeQueue.Enqueue(type);
        _onAddDialog.OnNext(Unit.Default);
    }

    public bool TryDequeue(out DialogEventType? dialogEvent)
    {
        if (_dialogEventTypeQueue.Count > 0)
        {
            dialogEvent = _dialogEventTypeQueue.Dequeue();
            return true;
        }

        dialogEvent = null;
        return false;
    }

    public bool HasDialog()
    {
        return _dialogEventTypeQueue.Count > 0;
    }
}
