using PoC03B.Shared.Enums;
using PoC03B.Shared.Models;
using System.ComponentModel;

namespace PoC03B.Client.ViewModels
{
    public interface IFormLayoutViewModel
    {
        event PropertyChangedEventHandler PropertyChanged;

        string FormId { get; }
        int RowsCount { get; }
        Guid? SelectedId { get; set; }
        string DragTypeName { set; }
        Guid DragID { set; }
        FormState State { get; set; }

        void AddRow();
        void RemoveRow();
        List<FormComponent> GetFormComponentsByRow(int rowId);
        List<ComponentParameter> GetParametersComponent();
        bool CheckState(FormState state);

        void NewForm();
        Task SaveForm();
        Task LoadForm(string id, FormState state);
        void RestoreForm(FormState state);
        Task<List<FormHistory>?> LoadHistory();

        void ProcessOperation(FieldOperation mainOperation, Guid? idOriginComponent, Guid? idTargetComponent);

        IDictionary<string, object> GetComponentParameters(List<ComponentParameter> componentParameters);
    }
}