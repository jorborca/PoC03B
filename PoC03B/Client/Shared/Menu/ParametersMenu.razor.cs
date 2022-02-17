namespace PoC03B.Client.Shared.Menu;

using Microsoft.AspNetCore.Components;
using PoC03B.Client.ViewModels;

public partial class ParametersMenu
{
    [CascadingParameter(Name = "FormLayoutViewModel")]
    IFormLayoutViewModel FormLayoutViewModel { get; set; }

}