public class OtherInputData : IInputUIDataCommand
{
    public InputUIMovableData GetInputUIMovableData()
    {
        return new InputUIMovableData(
            false,
            false,
            false,
            false
            );
    }
}
