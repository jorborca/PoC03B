namespace PoC03B.Client.Shared;

using Microsoft.AspNetCore.Components;
using MudBlazor;
using PoC03B.Client.ViewModels;
using PoC03B.Shared.Enums;

public partial class MainLayout
{
    [Inject] protected IFormDesignerViewModel FormDesignerViewModel { get; set; }
    [Inject] protected NavigationManager NavigationManager { get; set; }
    [Inject] protected ISnackbar SnackBar { get; set; }

    private void OnClick_NewForm()
    {
        FormDesignerViewModel.NewForm();

        NavigationManager.NavigateTo($"/Edit");
    }

    private void OnClick_SelectForm()
    {
        FormDesignerViewModel.SetState(FormState.Selection);

        NavigationManager.NavigateTo($"/");
    }

    private async Task OnClick_SaveForm()
    {
        await FormDesignerViewModel.SaveForm();

        SnackBar.Add($"Formulario guardado.");
    }

    private void OnClick_DesignState()
    {
        FormDesignerViewModel.SetState(FormState.Design);
    }

    private void OnClick_ViewState()
    {
        FormDesignerViewModel.SetState(FormState.View);
    }

    //private async Task OnClick_LoadForm()
    //{
    //    //var response = await HttpClient.GetFromJsonAsync<FormDesigner>("templates/load/DynamicForm_1");

    //    //if (response == null) return;

    //    //FormDesignerData = response;
    //    //FormDesignerService.RestoreForm();

    //    //FormDesignerService.SetState(FormState.Design);
    //}
}