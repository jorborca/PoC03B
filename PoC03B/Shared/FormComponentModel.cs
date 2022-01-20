using System.Text.Json.Serialization;

namespace PoC03B.Shared;

public class FormComponentModel
{
    public Guid Id { get; set; }
    public int RowId { get; set; }
    public int ColId { get; set; }
    public Type? ComponentType { get; set; }
    //[JsonPropertyName("typeName")]
    //[JsonConverter(typeof(Type))]
    public string? TypeName { get; set; }
    public IDictionary<string, object> Parameters { get; set; }
    public int Xs { get; set; }

}