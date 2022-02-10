using PoC03B.Shared.Enums;
using PoC03B.Shared.Models;

namespace PoC03B.Client.ViewModels
{
    public interface IFormDesignerViewModel
    {
        void AddRow();
        void RemoveRow();
        int GetRowsCount();
        List<FormComponent> GetFormComponentsByRow(int rowId);

        void SetDragID(Guid id);
        void SetDragTypeName(string typeName);

        FormState GetState();
        void SetState(FormState state);
        bool CheckState(FormState state);

        void NewForm();
        Task SaveForm();
        Task LoadForm(string id);
        Task<List<FormHistory>?> LoadHistory();

        void ProcessOperation(FieldOperation mainOperation, Guid? idOriginComponent, Guid? idTargetComponent);
    }
}