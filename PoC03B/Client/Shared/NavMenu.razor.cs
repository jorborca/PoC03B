namespace PoC03B.Client.Shared;

using Microsoft.AspNetCore.Components;
using PoC03B.Client.ViewModels;

public partial class NavMenu
{
    [CascadingParameter(Name = "FormLayoutViewModel")]
    IFormLayoutViewModel FormLayoutViewModel { get; set; }

    private void OnDragStart(string typeName)
    {
        FormLayoutViewModel.SetDragTypeName(typeName);
    }

    private async Task OnClick_AddRow()
    {
        FormLayoutViewModel.AddRow();
        StateHasChanged();
    }

    private void OnClick_RemoveRow()
    {
        FormLayoutViewModel.RemoveRow();
    }

}