using Microsoft.AspNetCore.Components;
using MudBlazor;
using PoC03B.Shared.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace PoC03B.Client.Pages
{
    public partial class DynamicFormComponent
    {
        [Inject] HttpClient Http { get; set; }

        //dynamic ViewModel { get; set; }

        List<FormComponent> formComponents = new();
        Action<string, string, object>? formAction { get; set; }

        private int MaxRowId() => formComponents.Max(x => x.RowId);
        private List<FormComponent> GetFormComponentsByRow(int rowId) => formComponents.Where(x => x.RowId == rowId).ToList();

        protected async override Task OnInitializedAsync()
        {
            formAction = OnDynamicForm_Action;

            await GetDynamicForm();
        }

        private async Task GetDynamicForm()
        {
            //WebHostEnvironment.WebRootPath
            //formComponents = await httpClient.GetFromJsonAsync<List<FormComponentModel>>("/DynamicForm_0.json");
            var response = await Http.GetFromJsonAsync<FormDesigner>("templates/load/DynamicForm_0");

            if (response == null) return;

            formComponents = response.Items;

            if (formComponents == null) return;

            foreach (var formComponent in formComponents)
            {
                formComponent.ComponentType = Type.GetType($"{formComponent.TypeName}");

                if (formComponent?.Parameters != null)
                {
                    foreach (var parameter in formComponent.Parameters)
                    {
                        string key = parameter.Key;
                        JsonElement jsonElement = (JsonElement)parameter.Value;

                        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(jsonElement.GetString()))
                        {
                            formComponent.Parameters.Remove(key);
                            continue;
                        }

                        object? newParameterType = key switch
                        {
                            "Visible" => jsonElement.GetBoolean(),
                            _ => jsonElement.GetString()
                        };

                        if (newParameterType == null)
                        {
                            formComponent.Parameters.Remove(key);
                            continue;
                        }

                        formComponent.Parameters[key] = newParameterType;
                    }
                }
            }
        }

        private void OnDynamicForm_Action(string senderId, string eventType, object data)
        {
            string action = $"{senderId}_{eventType}";

            switch (action)
            {
                case "btnAceptar_OnClick":
                    snackBar.Add($"Action: {action}", Severity.Success);
                    break;

                case "btnCancelar_OnClick":
                    snackBar.Add($"Action: {action}", Severity.Error);
                    break;
            }
        }
    }
}
