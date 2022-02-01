using Microsoft.AspNetCore.Components;
using PoC03B.Shared.Enums;
using PoC03B.Shared.Models;
using System.Net;
using System.Net.Http.Json;

namespace PoC03B.Client.Pages;

public partial class Index
{
    [Inject] HttpClient Http { get; set; }
    [Inject] NavigationManager NavigationManager { get; set; }

    [CascadingParameter(Name = "FormDesignerData")]
    protected FormDesigner FormDesignerData { get; set; }

    public List<FormHistory> FormHistory { get; set; } = new();
    bool loading = true;

    protected override async Task OnInitializedAsync()
    {
        await LoadHistory();
        loading = false;
    }

    private void OnClick_EditTemplate(Guid id)
    {
        FormDesignerData.Rows = 1;
        FormDesignerData.State = FormState.Design;

        NavigationManager.NavigateTo($"/Edit/{id}");
    }

    private async Task LoadHistory()
    {
        var response = await Http.GetAsync("history/load");

        if(response.StatusCode != HttpStatusCode.OK)
        {
            return;
        }

        var formHistory = await response.Content.ReadFromJsonAsync<List<FormHistory>>();

        if(formHistory != null) FormHistory = formHistory;
    }
}