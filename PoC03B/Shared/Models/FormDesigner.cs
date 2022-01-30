using System.Text.Json.Serialization;

namespace PoC03B.Shared.Models
{
    public class FormDesigner
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Rows { get; set; }
        [JsonIgnore]
        public string? DragByTypeName { get; set; }
        [JsonIgnore] 
        public Guid? DragByID { get; set; }
        public List<FormComponent> Items { get; set; } = new();

        public FormDesigner Clone()
        {
            FormDesigner other = (FormDesigner)this.MemberwiseClone();
            other.Items = new List<FormComponent>(this.Items);
            return other;
            //return (FormDesignerModel)this.Clone();
        }
    }
}
