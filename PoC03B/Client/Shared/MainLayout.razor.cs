namespace PoC03B.Client.Shared;

using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PoC03B.Shared.Enums;
using PoC03B.Shared.Models;

public partial class MainLayout
{
    [Inject] NavigationManager NavigationManager { get; set; }
    [Inject] HttpClient Http { get; set; }
    [Inject] ISnackbar SnackBar { get; set; }

    protected FormDesigner FormDesignerData { get; set; } = new();
    
    protected override async Task OnInitializedAsync()
    {
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

    private void OnClick_SelectTemplate()
    {
        FormDesignerData.State = FormState.Selection;

        NavigationManager.NavigateTo($"/");
    }

    private void OnClick_NewTemplate()
    {
        FormDesignerData = new FormDesigner
        {
            Id = Guid.NewGuid(),
            Name = "Demo",
            Description = "Demo",
            Rows = 1,
            State = FormState.Design
        };

        AddFormRow();

        NavigationManager.NavigateTo($"/Edit");
    }

    private async Task OnClick_SaveTemplate()
    {
        FormDesigner cleanTemplate = FormDesignerData.Clone();

        cleanTemplate.Items.RemoveAll(x => x.State != FieldState.Hold);
        await Http.PostAsJsonAsync("templates/save", cleanTemplate);

        await SaveHistory(cleanTemplate.Id, cleanTemplate.Name, cleanTemplate.Description);

        SnackBar.Add($"Se guardó la plantilla [{cleanTemplate.Name}]");
    }

    private void OnClick_DesignMode()
    {
        FormDesignerData.State = FormState.Design;
    }

    private void OnClick_ViewMode()
    {
        FormDesignerData.State = FormState.View;
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

    private async Task SaveHistory(Guid id, string name, string description)
    {
        bool Adding = false;
        List<FormHistory> formHistory = new();
        var response = await Http.GetAsync("history/load");

        switch(response.StatusCode)
        {
            case HttpStatusCode.OK:
                formHistory = await response.Content.ReadFromJsonAsync<List<FormHistory>>();

                if (formHistory.Any((x => x.Id == id)))
                {
                    var history = formHistory.First(x => x.Id == id);
                    history.SavedDate = DateTime.Now;
                }
                else
                {
                    Adding = true;
                }
                break;

            case HttpStatusCode.NotFound:
                Adding = true;
                break;

            default:
            case HttpStatusCode.BadRequest:
                return;
        }

        if(Adding)
        {
            formHistory.Add(new FormHistory()
            {
                Id = id,
                Name = name,
                Description = description,
                SavedDate = DateTime.Now
            });
        }

        await Http.PostAsJsonAsync("history/save", formHistory);
    }

}