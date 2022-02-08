using Microsoft.AspNetCore.Components;
using PoC03B.Client.ViewModels;
using PoC03B.Shared.Enums;
using PoC03B.Shared.Models;
using System.Net;
using System.Net.Http.Json;

namespace PoC03B.Client.Pages;

public partial class Index
{
    [Inject] protected FormDesignerViewModel FormDesignerViewModel { get; set; }
    [Inject] protected NavigationManager NavigationManager { get; set; }

    List<FormHistory>? FormHistory { get; set; } = new();

    bool loading = true;

    protected override async Task OnInitializedAsync()
    {
        FormHistory = await FormDesignerViewModel.LoadHistory();
        loading = false;
    }

    private void OnClick_EditForm(Guid id)
    {
        NavigationManager.NavigateTo($"/Edit/{id}");
    }
}