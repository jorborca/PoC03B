namespace PoC03B.Client.Pages.Designer.Dialogs;

using Microsoft.AspNetCore.Components;
using MudBlazor;
using PoC03B.Client.ViewModels;
using PoC03B.Shared.Enums;
using System.Threading.Tasks;

public partial class ToolsMenuDialog
{
    //[Inject] IFormLayoutViewModel FormLayoutViewModelService { get; set; }

    [CascadingParameter] 
    MudDialogInstance MudDialog { get; set; }

    //[CascadingParameter(Name = "FormLayoutViewModel")]
    [Parameter]
    public IFormLayoutViewModel FormLayoutViewModel { get; set; }

    //void Cancel() => MudDialog.Cancel();

    private void OnClick_ComponentType(string typeName)
    {
        FormLayoutViewModel.DragTypeName = typeName;
        FormLayoutViewModel.ProcessOperation(FieldOperation.Add, null, FormLayoutViewModel.SelectedId);
        MudDialog.Close(DialogResult.Ok(true));
    }

}