using Microsoft.AspNetCore.Components;
using PoC03B.Client.ViewModels;

namespace PoC03B.Client.Shared;

public partial class NavMenu
{
    [Inject] protected FormDesignerViewModel FormDesignerViewModel { get; set; }

    private void OnDragStart(string typeName)
    {
        FormDesignerViewModel.SetDragTypeName(typeName);
    }

    private void OnClick_AddRow()
    {
        FormDesignerViewModel.AddRow(1);
    }

    private void OnClick_RemoveRow()
    {
        FormDesignerViewModel.RemoveRow();
    }

}