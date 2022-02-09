using PoC03B.Shared.Enums;
using PoC03B.Shared.Models;

namespace PoC03B.Client.ViewModels
{
    public interface IFormDesignerViewModel
    {
        void AddRow(int increment);
        bool CheckState(FormState state);
        List<FormComponent> GetFormComponentsByRow(int rowId);
        int GetRowsCount();
        FormState GetState();
        Task LoadForm(string id);
        Task<List<FormHistory>?> LoadHistory();
        void NewForm();
        void ProcessOperation(FieldOperation mainOperation, Guid? idOriginComponent, Guid? idTargetComponent);
        void RemoveRow();
        Task SaveForm();
        void SetDragID(Guid id);
        void SetDragTypeName(string typeName);
        void SetState(FormState state);
    }
}