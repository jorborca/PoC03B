namespace PoC03B.Client.Pages.Renderer.Components;

using Microsoft.AspNetCore.Components;
using MudBlazor;
using PoC03B.Client.ViewModels;

public partial class DynamicFormComponent
{
    [Inject] protected IFormLayoutViewModel FormLayoutViewModel { get; set; }
    [Inject] protected ISnackbar Snackbar { get; set; }

    [Parameter] public string? FormID { get; set; }

    Action<string, string, object>? formAction { get; set; }

    protected async override Task OnInitializedAsync()
    {
        formAction = OnForm_Action;

        await FormLayoutViewModel.LoadForm("DynamicForm_0");
    }

    private void OnForm_Action(string senderId, string eventType, object data)
    {
        string action = $"{senderId}_{eventType}";

        switch (action)
        {
            case "btnAceptar_OnClick":
                Snackbar.Add($"Action: {action}", Severity.Success);
                break;

            case "btnCancelar_OnClick":
                Snackbar.Add($"Action: {action}", Severity.Error);
                break;
        }
    }
}