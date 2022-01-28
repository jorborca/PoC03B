using PoC03B.Shared.Enums;
using System.Text.Json.Serialization;

namespace PoC03B.Shared.Models;

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
    public int Sm { get; set; }
    public int Md { get; set; }
    public int Lg { get; set; }

    public FieldPosition Position { get; set; }
    public FieldState State { get; set; }

    //public FormComponentModel Clone()
    //{
    //    return (FormComponentModel)this.MemberwiseClone();
    //}
}