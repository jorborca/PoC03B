using System.Text.Json.Serialization;

namespace PoC03B.Shared;

public class FormComponentModel
{
    public Guid Id { get; set; }
    public int RowId { get; set; }
    public int ColId { get; set; }
    //[JsonPropertyName("ComponentType")]
    //[JsonConverter(typeof(Type))]
    [JsonIgnore]
    public Type? ComponentType { get; set; }
    public string? TypeName { get; set; }
    public IDictionary<string, object> Parameters { get; set; }
    public int Xs { get; set; }

}