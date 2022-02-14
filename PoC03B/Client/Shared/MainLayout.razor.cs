namespace PoC03B.Client.Shared;

using Microsoft.AspNetCore.Components;
using MudBlazor;
using PoC03B.Client.ViewModels;
using PoC03B.Shared.Enums;
using System.ComponentModel;

public partial class MainLayout
{
    [Inject] protected IFormLayoutViewModel FormLayoutViewModelService { get; set; }
    [Inject] protected NavigationManager NavigationManager { get; set; }
    [Inject] protected ISnackbar SnackBar { get; set; }

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

    private void OnClick_NewForm()
    {
        NavigationManager.NavigateTo($"/Edit");
        //StateHasChanged();
    }

    private void OnClick_SelectForm()
    {
        FormLayoutViewModelService.SetState(FormState.Selection);

        NavigationManager.NavigateTo($"/");
    }

    private async Task OnClick_SaveForm()
    {
        await FormLayoutViewModelService.SaveForm();

        SnackBar.Add($"Formulario guardado.");
    }

    private void OnClick_DesignMode()
    {
        NavigationManager.NavigateTo($"edit/{FormLayoutViewModelService.GetFormId()}");
    }

    private void OnClick_ViewMode()
    {
        NavigationManager.NavigateTo($"render/{FormLayoutViewModelService.GetFormId()}");
    }

}