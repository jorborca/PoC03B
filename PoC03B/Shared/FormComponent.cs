using System.Text.Json.Serialization;

namespace PoC03B.Shared;

public class FormComponent
{
    public string Id { get; set; }

    public Type TypeComponent { get; set; }

    //[JsonPropertyName("typeName")]
    //[JsonConverter(typeof(Type))]
    public string TypeName { get; set; }

    public IDictionary<string, object> Parameters { get; set; }

    public int xs { get; set; }

    public int RowId { get; set; }
}