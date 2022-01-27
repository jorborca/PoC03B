namespace PoC03B.Client.Shared;

using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using PoC03B.Shared.Enums;
using PoC03B.Shared.Models;

public partial class MainLayout
{
    [Inject] HttpClient Http { get; set; }

    private FormDesignerModel FormDesignerData { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        FormDesignerData.Name = "DynamicForm_1";
        AddFormRow();
    }

    private void OnAddRow()
    {
        AddFormRow();
    }

    private void OnRemoveRow()
    {
        RemoveFormRow();
    }

    private void AddFormRow()
    {
        FormDesignerData.Rows++;

        int rowId = FormDesignerData.Rows;

        if (!FormDesignerData.Items.Any(x => x.RowId == rowId))
        {
            for (int colId = 1; colId <= 12; colId++)
            {
                FormDesignerData.Items.Add(new FormComponentModel()
                {
                    Id = Guid.NewGuid(),
                    RowId = rowId,
                    ColId = colId,
                    Xs = 1,
                    Position = FieldPosition.CenterCenter,
                    State = FieldState.Empty
                });
            }
        }
    }

    private void RemoveFormRow()
    {
        if (FormDesignerData.Rows == 1) return;

        int rowId = FormDesignerData.Rows;

        if (!FormDesignerData.Items.Any(x => x.RowId == rowId && x.TypeName != null))
        {
            FormDesignerData.Items.RemoveAll(x => x.RowId == rowId);
            FormDesignerData.Rows--;
        }
    }

    private async Task OnSaveTemplate()
    {
        await Http.PostAsJsonAsync("templates/save", FormDesignerData);
    }

    private async Task OnLoadTemplate()
    {
        var response = await Http.GetFromJsonAsync<FormDesignerModel>("templates/load/DynamicForm_1");

        if (response == null) return;

        FormDesignerData = response;

        FormDesignerData.Items.Where(x => x.TypeName != null).ToList().ForEach(x => x.ComponentType = Type.GetType($"{x.TypeName}"));
    }
}
