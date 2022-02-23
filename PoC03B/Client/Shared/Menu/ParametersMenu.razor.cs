namespace PoC03B.Client.Shared.Menu;

using Microsoft.AspNetCore.Components;
using MudBlazor;
using PoC03B.Client.ViewModels;

public partial class ParametersMenu
{
    [CascadingParameter(Name = "FormLayoutViewModel")]
    IFormLayoutViewModel FormLayoutViewModel { get; set; }

    //private Type GetComponentType(object value)
    //{
    //    Type componentType = new MudTextField<string>().GetType();

    //    if (value.GetType().Equals(typeof(string)))
    //    {
    //        componentType = new MudTextField<string>().GetType();
    //    }

    //    return componentType;
    //}

    //private IDictionary<string, object> GetComponentParameters(object value)
    //{
    //    IDictionary<string, object> componentParameters = new Dictionary<string, object>();

    //    if (value.GetType().Equals(typeof(string)))
    //    {
    //        componentParameters.Add("Variant", Variant.Outlined);


    //    }

    //    return componentParameters;
    //}

}