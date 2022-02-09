namespace PoC03B.Client.Pages;

using Microsoft.AspNetCore.Components;
using MudBlazor;
using PoC03B.Client.ViewModels;
using PoC03B.Shared.Enums;
using PoC03B.Shared.Models;
using System.Net.Http.Json;
using System.Threading.Tasks;

public partial class EditFormComponent
{
    [Inject] protected IFormDesignerViewModel FormDesignerViewModel { get; set; }
    [Inject] protected ISnackbar Snackbar { get; set; }

    [Parameter] public string? idForm { get; set; }
    
    FieldOperation MainOperation = FieldOperation.Move;
    string dropClass = "";

    protected override async Task OnInitializedAsync()
    {
        if (idForm != null)
        {
            await FormDesignerViewModel.LoadForm(idForm);
        }
    }

    private void OnDragStart(FieldOperation operation, Guid idOriginComponent)
    {
        MainOperation = operation;
        FormDesignerViewModel.SetDragID(idOriginComponent);
    }

    private void OnDragEnter(Guid id)
    {
        //if (AllowedStatuses != null && !AllowedStatuses.Contains(Container.Payload.Status))
        //{
        //    dropClass = "no-drop";
        //}
        //else
        //{
        //    dropClass = "can-drop";
        //}
    }

    private void OnDragOver()
    {

    }

    private void OnDragLeave()
    {
        //dropClass = "";
    }

    private void OnDrop(Guid idTargetComponent)
    {
        FormDesignerViewModel.ProcessOperation(FieldOperation.Move, null, idTargetComponent);
    }

    private void OnClick_MainOperation(Guid idOriginComponent)
    {
        FormDesignerViewModel.ProcessOperation(MainOperation, idOriginComponent, null);
        MainOperation = FieldOperation.Move;
    }

    private void OnChange_MainOperation()
    {
        MainOperation++;
        if (MainOperation > FieldOperation.Split) MainOperation = 0;
    }

    private void OnMouseLeave_MainOperation()
    {
        MainOperation = FieldOperation.Move;
    }

    private string GetMainOperationColor()
    {
        return MainOperation switch
        {
            FieldOperation.Move => "mud-theme-info",
            FieldOperation.Resize => "mud-theme-info",
            FieldOperation.Delete => "mud-theme-error",
            FieldOperation.Expand => "mud-theme-success",
            FieldOperation.Split => "mud-theme-dark",
        };
    }

    private string GetPositionClass(FieldPosition position)
    {
        string verticalPosition = "align-items-top";
        string horizontalPosition = "mud-typography-align-left";

        if (position.ToString().Contains("Top")) verticalPosition = "align-items-top";
        if (position.ToString().Contains("Medium")) verticalPosition = "align-items-center";
        if (position.ToString().Contains("Bottom")) verticalPosition = "align-items-bottom";

        if (position.ToString().Contains("Left")) horizontalPosition = "mud-typography-align-left";
        if (position.ToString().Contains("Center")) horizontalPosition = "mud-typography-align-center";
        if (position.ToString().Contains("Right")) horizontalPosition = "mud-typography-align-right";

        return $"{horizontalPosition} {verticalPosition}";
    }

}