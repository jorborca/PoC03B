namespace PoC03B.Client.Shared;

using Microsoft.AspNetCore.Components;
using PoC03B.Client.ViewModels;

public partial class NavMenuTools
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
    }

    private void OnClick_RemoveRow()
    {
        FormLayoutViewModel.RemoveRow();
    }

}