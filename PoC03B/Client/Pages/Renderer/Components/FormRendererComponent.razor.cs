namespace PoC03B.Client.Pages.Renderer.Components;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using PoC03B.Client.ViewModels;
using PoC03B.Shared.Models;
using MudBlazor;

public partial class FormRendererComponent
{
    [Inject] protected IFormDesignerViewModel FormDesignerViewModel { get; set; }
    [Inject] protected ISnackbar Snackbar { get; set; }

    [Parameter] public string? FormID { get; set; }

    Action<string, string, object>? formAction { get; set; }

    protected async override Task OnInitializedAsync()
    {
        formAction = OnForm_Action;

        await FormDesignerViewModel.LoadForm("DynamicForm_0");

        //await GetForm();
    }

    private async Task GetForm()
    {
        //WebHostEnvironment.WebRootPath
        //formComponents = await httpClient.GetFromJsonAsync<List<FormComponentModel>>("/DynamicForm_0.json");

        //formComponents = response.Items;

        //if (formComponents == null) return;

        //foreach (var formComponent in formComponents)
        //{
        //    formComponent.ComponentType = Type.GetType($"{formComponent.TypeName}");

        //    if (formComponent?.Parameters != null)
        //    {
        //        foreach (var parameter in formComponent.Parameters)
        //        {
        //            string key = parameter.Key;
        //            JsonElement jsonElement = (JsonElement)parameter.Value;

        //            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(jsonElement.GetString()))
        //            {
        //                formComponent.Parameters.Remove(key);
        //                continue;
        //            }

        //            object? newParameterType = key switch
        //            {
        //                "Visible" => jsonElement.GetBoolean(),
        //                _ => jsonElement.GetString()
        //            };

        //            if (newParameterType == null)
        //            {
        //                formComponent.Parameters.Remove(key);
        //                continue;
        //            }

        //            formComponent.Parameters[key] = newParameterType;
        //        }
        //    }
        //}
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