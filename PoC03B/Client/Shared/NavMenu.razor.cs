using Microsoft.AspNetCore.Components;
using PoC03B.Client.ViewModels;

namespace PoC03B.Client.Shared;

public partial class NavMenu
{
    [Inject] protected IFormDesignerViewModel FormDesignerViewModel { get; set; }

    private void OnDragStart(string typeName)
    {
        FormDesignerViewModel.SetDragTypeName(typeName);
    }

    private async Task OnClick_AddRow()
    {
        FormDesignerViewModel.AddRow();
        StateHasChanged();
    }

    private void OnClick_RemoveRow()
    {
        FormDesignerViewModel.RemoveRow();
    }

}