namespace PoC03B.Client.Shared;

using Microsoft.AspNetCore.Components;
using MudBlazor;
using PoC03B.Client.ViewModels;
using PoC03B.Shared.Enums;
using System.ComponentModel;

public partial class MainLayout
{
    [Inject] IFormLayoutViewModel FormLayoutViewModelService { get; set; }
    [Inject] NavigationManager NavigationManager { get; set; }
    [Inject] ISnackbar SnackBar { get; set; }

    #region Reactive ViewModel Events
    
    protected override async Task OnInitializedAsync()
    {
        FormLayoutViewModelService.PropertyChanged += async (sender, e) => {
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        };
        await base.OnInitializedAsync();
    }

    async void OnPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
    {
        await InvokeAsync(() =>
        {
            StateHasChanged();
        });
    }

    public void Dispose()
    {
        FormLayoutViewModelService.PropertyChanged -= OnPropertyChangedHandler;
    }

    #endregion

    private async Task OnClick_AddRow()
    {
        FormLayoutViewModelService.AddRow();
    }

    private void OnClick_RemoveRow()
    {
        FormLayoutViewModelService.RemoveRow();
    }

    private void OnClick_SelectForm()
    {
        FormLayoutViewModelService.State = FormState.Selection;

        NavigationManager.NavigateTo($"/");
    }

    private async Task OnClick_SaveForm()
    {
        await FormLayoutViewModelService.SaveForm();
        SnackBar.Add($"Formulario guardado.");
    }

    private void OnClick_DesignMode()
    {
        NavigationManager.NavigateTo($"edit/{FormLayoutViewModelService.FormId}");
    }

    private async Task OnClick_ViewMode()
    {
        await FormLayoutViewModelService.SaveForm();
        SnackBar.Add($"Formulario actualizado.");
        NavigationManager.NavigateTo($"view/{FormLayoutViewModelService.FormId}");
    }

}