namespace PoC03B.Client.Shared;

using Microsoft.AspNetCore.Components;
using PoC03B.Client.ViewModels;

public partial class NavMenuProperties
{
    [CascadingParameter(Name = "FormLayoutViewModel")]
    IFormLayoutViewModel FormLayoutViewModel { get; set; }

}