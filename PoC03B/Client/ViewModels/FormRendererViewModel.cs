namespace PoC03B.Client.ViewModels;

using PoC03B.Client.Services;
using PoC03B.Shared.Enums;
using PoC03B.Shared.Models;

public class FormRendererViewModel : IFormRendererViewModel
{
    private FormLayout _FormLayout { get; set; }
    private IFormApiService _FormApiService { get; set; }

    public FormRendererViewModel(IFormApiService formDesignerService)
    {
        this._FormLayout = new();
        this._FormApiService = formDesignerService;
    }

    public async Task LoadForm(string idForm)
    {
        int ixItem = 0;
        int joins = 0;

        var response = await _FormApiService.GetForm(idForm);

        if (response == null) return;

        _FormLayout = response;

        //FormDesignerData.Items.Where(x => x.State == FieldState.Hold).ToList()
        //.ForEach(x => x.ComponentType = Type.GetType($"{x.TypeName}"));

        for (int rowId = 1; rowId <= _FormLayout.Rows; rowId++)
        {
            for (int colId = 1; colId <= 12; colId++)
            {
                if (_FormLayout.Items.Any(x => x.RowId == rowId && x.ColId == colId))
                {
                    var item = _FormLayout.Items.Single(x => x.RowId == rowId && x.ColId == colId);
                    item.ComponentType = Type.GetType($"{item.TypeName}");

                    if (item.Xs > 1) joins = item.Xs;
                }
                else
                {
                    ixItem = (rowId - 1) * 12 + (colId - 1);

                    _FormLayout.Items.Insert(ixItem, new FormComponent()
                    {
                        Id = Guid.NewGuid(),
                        RowId = rowId,
                        ColId = colId,
                        Xs = 1,
                        Sm = 1,
                        Md = 1,
                        Lg = 1,
                        Position = FieldPosition.MediumCenter,
                        State = joins > 1 ? FieldState.Disabled : FieldState.Empty
                    });

                    joins--;
                }
            }
        }
    }

    private async Task LoadForm()
    {
        //WebHostEnvironment.WebRootPath
        //formComponents = await httpClient.GetFromJsonAsync<List<FormComponentModel>>("/DynamicForm_0.json");

        //formComponents = response.Items;

        //if (formComponents == null) return;

        //foreach (var formComponent in formComponents)
        //{
        //    formComponent.ComponentType = Type.GetType($"{formComponent.TypeName}");

        //    if (formComponent?.Parameters != null)
        //    {
        //        foreach (var parameter in formComponent.Parameters)
        //        {
        //            string key = parameter.Key;
        //            JsonElement jsonElement = (JsonElement)parameter.Value;

        //            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(jsonElement.GetString()))
        //            {
        //                formComponent.Parameters.Remove(key);
        //                continue;
        //            }

        //            object? newParameterType = key switch
        //            {
        //                "Visible" => jsonElement.GetBoolean(),
        //                _ => jsonElement.GetString()
        //            };

        //            if (newParameterType == null)
        //            {
        //                formComponent.Parameters.Remove(key);
        //                continue;
        //            }

        //            formComponent.Parameters[key] = newParameterType;
        //        }
        //    }
        //}
    }

}