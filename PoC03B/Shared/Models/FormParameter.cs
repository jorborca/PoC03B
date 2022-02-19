namespace PoC03B.Shared.Models;

public class FormParameter
{
    public string Name { get; set; }
    public string TypeName { get; set; }
    public object? Value { get; set; }

    public static implicit operator List<object>(FormParameter v)
    {
        throw new NotImplementedException();
    }
}