namespace PoC03B.Shared
{
    public class FormDesignerModel
    {
        public int Rows { get; set; }
        public string? DragByTypeName { get; set; }
        public Guid? DragByID { get; set; }
        public List<FormComponentModel> Items { get; set; } = new();
    }
}
