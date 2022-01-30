namespace PoC03B.Client.Shared;

using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PoC03B.Shared.Enums;
using PoC03B.Shared.Models;

public partial class MainLayout
{
    [Inject] HttpClient Http { get; set; }
    [Inject] ISnackbar SnackBar { get; set; }

    private FormDesigner FormDesignerData { get; set; }
    bool InDesign = true;

    protected override async Task OnInitializedAsync()
    {
        OnClick_NewTemplate();
    }

    private void OnClick_AddRow()
    {
        FormDesignerData.Rows++;
        AddFormRow();
    }

    private void OnClick_RemoveRow()
    {
        RemoveFormRow();
    }

    private void OnClick_NewTemplate()
    {
        FormDesignerData = new FormDesigner
        {
            Id = Guid.NewGuid(),
            Code = "DynamicForm_1",
            Name = "DynamicForm_1",
            Rows = 1,
        };

        AddFormRow();
    }

    private async Task OnClick_SaveTemplate()
    {
        FormDesigner cleanTemplate = FormDesignerData.Clone();

        cleanTemplate.Items.RemoveAll(x => x.State != FieldState.Hold);
        await Http.PostAsJsonAsync("templates/save", cleanTemplate);

        await SaveHistory(cleanTemplate.Id, cleanTemplate.Name);

        SnackBar.Add($"Se guardó la plantilla [{cleanTemplate.Name}]");
    }

    private void OnClick_InDesign()
    {
        InDesign = !InDesign;
    }

    private void AddFormRow()
    {
        int rowId = FormDesignerData.Rows;

        if (!FormDesignerData.Items.Any(x => x.RowId == rowId))
        {
            for (int colId = 1; colId <= 12; colId++)
            {
                FormDesignerData.Items.Add(new FormComponent()
                {
                    Id = Guid.NewGuid(),
                    RowId = rowId,
                    ColId = colId,
                    Xs = 1,
                    Sm = 1,
                    Md = 1,
                    Lg = 1,
                    Position = FieldPosition.MediumCenter,
                    State = FieldState.Empty
                });
            }
        }
    }

    private void RemoveFormRow()
    {
        if (FormDesignerData.Rows == 1) return;

        int rowId = FormDesignerData.Rows;

        if (!FormDesignerData.Items.Any(x => x.RowId == rowId && x.State == FieldState.Hold))
        {
            FormDesignerData.Items.RemoveAll(x => x.RowId == rowId);
            FormDesignerData.Rows--;
        }
    }

    private async Task SaveHistory(Guid id, string name)
    {
        var formHistory = await Http.GetFromJsonAsync<List<FormHistory>>("history/load");
        if (formHistory != null) return;

        var history = formHistory.First(x => x.Id == id);

        if(history == null)
        {
            formHistory.Add(new FormHistory()
            {
                Id = id,
                Name = name,
                SavedDate = DateTime.Now
            });
        }
        else
        {
            history.SavedDate = DateTime.Now;
        }

        await Http.PostAsJsonAsync("history/save", formHistory);
    }
}