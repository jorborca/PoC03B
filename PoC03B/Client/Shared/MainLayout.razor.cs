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

    private FormDesignerModel FormDesignerData { get; set; }
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
        FormDesignerData = new FormDesignerModel
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
        FormDesignerModel cleanTemplate = FormDesignerData.Clone();

        cleanTemplate.Items.RemoveAll(x => x.State != FieldState.Hold);

        await Http.PostAsJsonAsync("templates/save", cleanTemplate);

        SnackBar.Add($"Se guardó la plantilla [{cleanTemplate.Name}]");
    }

    private async Task OnClick_LoadTemplate()
    {
        var response = await Http.GetFromJsonAsync<FormDesignerModel>("templates/load/DynamicForm_1");

        if (response == null) return;

        FormDesignerData = response;

        RestoreForm();
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
                FormDesignerData.Items.Add(new FormComponentModel()
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

    private void RestoreForm()
    {
        int rowId = FormDesignerData.Rows;

        FormDesignerData.Items.Where(x => x.TypeName != null).ToList().ForEach(x => x.ComponentType = Type.GetType($"{x.TypeName}"));


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
                    Sm = 1,
                    Md = 1,
                    Lg = 1,
                    Position = FieldPosition.MediumCenter,
                    State = FieldState.Empty
                });
            }
        }
    }
}