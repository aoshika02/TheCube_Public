public interface ITileObj 
{
    public UseType UseType { get; set; }
}

public enum UseType
{
    None,
    Stage,
    Select,
    Preview
}