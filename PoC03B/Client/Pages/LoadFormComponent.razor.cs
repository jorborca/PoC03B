using Microsoft.AspNetCore.Components;
using PoC03B.Shared.Models;
using System.Net.Http.Json;

namespace PoC03B.Client.Pages;

public partial class LoadFormComponent
{
    [Inject] HttpClient Http { get; set; }
    [Inject] NavigationManager NavigationManager { get; set; }

    public List<FormHistory> FormHistory { get; set; } = new();
    bool loading = true;

    protected override async Task OnInitializedAsync()
    {
        await LoadHistory();
        loading = false;
    }

    private async Task OnClick_EditTemplate(Guid id)
    {
        NavigationManager.NavigateTo($"/{id}", true);
    }

    private async Task LoadHistory()
    {
        var response = await Http.GetFromJsonAsync<List<FormHistory>>("history/load");

        if (response == null) return;

        FormHistory = response;
    }

}