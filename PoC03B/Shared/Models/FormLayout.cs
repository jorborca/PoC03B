using PoC03B.Shared.Enums;
using System.Text.Json.Serialization;

namespace PoC03B.Shared.Models
{
    public class FormLayout
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Rows { get; set; }
        
        [JsonIgnore]
        public string? DragByTypeName { get; set; }
        
        [JsonIgnore]
        public Guid? DragById { get; set; }

        [JsonIgnore]
        public Guid? SelectedId { get; set; }
        
        public List<FormComponent> Items { get; set; } = new();
        
        [JsonIgnore]
        public FormState State { get; set; }

        public FormLayout Clone()
        {
            FormLayout other = (FormLayout)this.MemberwiseClone();
            other.Items = new List<FormComponent>(this.Items);
            return other;
            //return (FormDesignerModel)this.Clone();
        }
    }
}
