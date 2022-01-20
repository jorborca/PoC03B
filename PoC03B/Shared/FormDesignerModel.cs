namespace PoC03B.Shared
{
    public class FormDesignerModel
    {
        public int Rows { get; set; }
        public string DragTypeName { get; set; }

        public List<FormComponentModel> Items { get; set; } = new();
    }
}
