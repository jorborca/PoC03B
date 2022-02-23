namespace PoC03B.Client.ViewModels;

using PoC03B.Client.Services;
using PoC03B.Shared.Enums;
using PoC03B.Shared.Models;
using System.ComponentModel;
using System.Text.Json;

public class FormLayoutViewModel : BaseViewModel, IFormLayoutViewModel
{
    private FormLayout _FormLayout { get; set; }
    private IFormApiService _FormApiService { get; set; }

    public FormLayoutViewModel(IFormApiService formDesignerService)
    {
        this._FormLayout = new();
        this._FormApiService = formDesignerService;
    }

    #region Properties

    public string FormId
    {
        get { return _FormLayout.Id.ToString(); }
    }

    public Guid? SelectedId
    {
        get { return _FormLayout.SelectedId; }
        set
        {
            IsBusy = true;
            _FormLayout.SelectedId = value;
            OnPropertyChanged(nameof(_FormLayout.SelectedId));
            IsBusy = false;
        }
    }

    public int RowsCount
    {
        get { return _FormLayout.Rows; }
    }

    public string DragTypeName
    {
        set { _FormLayout.AddByTypeName = value; }
    }

    public Guid DragID
    {
        set { _FormLayout.DragById = value; }
    }

    public FormState State
    {
        get { return _FormLayout.State; }
        set { _FormLayout.State = value; }
    }

    #endregion

    #region Methods

    public void AddRow()
    {
        IsBusy = true;

        _FormLayout.Rows++;

        if (!_FormLayout.Items.Any(x => x.RowId == _FormLayout.Rows))
        {
            for (int colId = 1; colId <= 12; colId++)
            {
                _FormLayout.Items.Add(new FormComponent()
                {
                    Id = Guid.NewGuid(),
                    RowId = _FormLayout.Rows,
                    ColId = colId,
                    Xs = 1,
                    Sm = 1,
                    Md = 1,
                    Lg = 1,
                    Parameters = new List<ComponentParameter>(),
                    Position = FieldPosition.MediumCenter,
                    State = FieldState.Empty
                });
            }
        }

        OnPropertyChanged(nameof(_FormLayout.Items));
        IsBusy = false;
    }

    public void RemoveRow()
    {
        IsBusy = true;

        if (_FormLayout.Rows == 1) return;

        int rowId = _FormLayout.Rows;

        if (!_FormLayout.Items.Any(x => x.RowId == rowId && x.State == FieldState.Hold))
        {
            _FormLayout.Items.RemoveAll(x => x.RowId == rowId);
            _FormLayout.Rows--;
        }

        OnPropertyChanged(nameof(_FormLayout.Items));
        IsBusy = false;
    }

    public bool CheckState(FormState state)
    {
        return _FormLayout.State == state;
    }

    public List<FormComponent> GetFormComponentsByRow(int rowId)
    {
        return _FormLayout.Items.Where(x => x.RowId == rowId && x.State != FieldState.Disabled).ToList();
    }

    public void ProcessOperation(FieldOperation mainOperation, Guid? idOriginComponent, Guid? idTargetComponent)
    {
        FormComponent originComponent = new();
        FormComponent targetComponent = new();

        IsBusy = true;

        if (mainOperation != FieldOperation.Select)
        {
            if (_FormLayout.DragById != null)
            {
                idOriginComponent = _FormLayout.DragById;
                _FormLayout.DragById = null;
            }

            if (idOriginComponent != null)
            {
                originComponent = _FormLayout.Items.Single(x => x.Id == idOriginComponent);
            }

            if (idTargetComponent != null)
            {
                targetComponent = _FormLayout.Items.Single(x => x.Id == idTargetComponent);
            }
        }

        switch (mainOperation)
        {
            case FieldOperation.Select:
                //SetValue(ref _FormLayout.SelectedId, idOriginComponent);
                //_FormLayout.SelectedId = idOriginComponent;
                SelectedId = idOriginComponent;
                break;

            case FieldOperation.Add:
                if (_FormLayout.AddByTypeName == null) return;

                targetComponent.TypeName = _FormLayout.AddByTypeName;
                Type? componentType = Type.GetType($"{_FormLayout.AddByTypeName}");
                targetComponent.ComponentType = componentType;

                targetComponent.Parameters = GetMockParameters();

                //targetComponent.Parameters = new Dictionary<string, object>();

                //formParameter.ForEach(x =>
                //{
                //    targetComponent.Parameters.Add(
                //        x.Name,
                //        GetObjectByType(x.TypeName, x.Value)
                //    );
                //});

                //try
                //{
                //    //var parameters = componentType.GetProperties();
                //    //var test = TypeDescriptor.GetProperties(componentType)["Label"].GetValue(componentType);

                //    //foreach (var propertyInfo in targetComponent.ComponentType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                //    //{
                //    //    string name = propertyInfo.Name;
                //    //    string type = propertyInfo.PropertyType.FullName;
                //    //    if (name == "Label")
                //    //    {
                //    //        object? value = propertyInfo.GetValue(targetComponent.Parameters);
                //    //    }
                //    //}
                //    //["Id"].GetValue(componentType);

                //    //parameters.ToList().ForEach(x =>
                //    //{
                //    //    //targetComponent.Parameters.Add(new FormParameter()
                //    //    //{
                //    //    //    Name = x.Name,
                //    //    //    TypeName = x.PropertyType.FullName,
                //    //    //    //Value = string.Empty
                //    //    //    //x.GetValue(targetComponent.ComponentType)
                //    //    //});
                //    //    //if (x.Name != "Attributes")
                //    //    //{
                //    //    //    targetComponent.Parameters.Add(                              
                //    //    //        x.Name,
                //    //    //        x.GetValue(targetComponent, null)
                //    //    //        //x.PropertyType.FullName
                //    //    //        //x.GetValue(targetComponent.ComponentType)
                //    //    //    );
                //    //    //}
                //    //});
                //}
                //catch (Exception ex)
                //{
                //    throw;
                //}

                targetComponent.State = FieldState.Hold;
                _FormLayout.AddByTypeName = null;
                break;

            case FieldOperation.Move:
                Type? backupComponentType = targetComponent.ComponentType;
                string? backupTypeName = targetComponent.TypeName;
                List<ComponentParameter> backupParameters = targetComponent.Parameters;

                targetComponent.TypeName = originComponent.TypeName;
                targetComponent.ComponentType = originComponent.ComponentType;
                targetComponent.Parameters = originComponent.Parameters;
                targetComponent.State = FieldState.Hold;

                RestoreField(originComponent, backupComponentType, backupTypeName, backupParameters);
                break;

            case FieldOperation.Resize:
                //Snackbar.Add($"Origin: {originComponent.ColId}-{originComponent.Xs}");
                //Snackbar.Add($"Target: {targetComponent.ColId}-{targetComponent.Xs}");

                // Join fields
                List<FormComponent> itemsToJoin = _FormLayout.Items.Where(
                    x => x.RowId == originComponent.RowId
                    && x.State == FieldState.Empty
                    && x.ColId > originComponent.ColId
                    && x.ColId <= targetComponent.ColId
                ).ToList();

                itemsToJoin.ForEach(x =>
                {
                    x.State = FieldState.Disabled;
                    originComponent.Xs++;
                });
                break;

            case FieldOperation.Delete: // Eliminar Item
                RestoreField(originComponent, null, null, new List<ComponentParameter>());
                break;

            case FieldOperation.Expand:
                var limitComponent = _FormLayout.Items.FirstOrDefault(
                    x => x.RowId == originComponent.RowId
                    && x.ColId > originComponent.ColId
                    && x.State == FieldState.Hold,
                    new FormComponent
                    {
                        ColId = 12
                    }
                );

                List<FormComponent> itemsToExpand = _FormLayout.Items.Where(
                    x => x.RowId == originComponent.RowId
                    && x.ColId > originComponent.ColId
                    && x.ColId <= limitComponent.ColId
                    && x.State == FieldState.Empty
                ).ToList();

                itemsToExpand.ForEach(x =>
                {
                    x.State = FieldState.Disabled;
                    originComponent.Xs++;
                });
                break;

            case FieldOperation.Split:
                if (originComponent.Xs > 1)
                {
                    _FormLayout.Items.Where(
                        x => x.RowId == originComponent.RowId
                        && x.ColId > originComponent.ColId
                        && x.ColId <= originComponent.ColId + originComponent.Xs)
                    .ToList().ForEach(x =>
                    {
                        originComponent.Xs = 1;
                        x.State = FieldState.Empty;
                    });
                }
                break;
        }

        OnPropertyChanged(nameof(_FormLayout.Items));
        IsBusy = false;
    }

    private void RestoreField(FormComponent originComponent, Type? newComponentType, string? newTypeName, List<ComponentParameter> parameters)
    {
        // Restore and Split the collapsed fields
        List<FormComponent> itemsToSplit = _FormLayout.Items.Where(
            x => x.RowId == originComponent.RowId
            && x.State == FieldState.Disabled
            && x.ColId > originComponent.ColId
            && x.ColId <= originComponent.ColId + originComponent.Xs
        ).ToList();

        itemsToSplit.ForEach(x =>
        {
            x.State = FieldState.Empty;
        });

        originComponent.ComponentType = newComponentType;
        originComponent.TypeName = newTypeName;
        originComponent.Parameters = parameters;
        originComponent.State = FieldState.Empty;
        originComponent.Xs = 1;
    }

    public List<ComponentParameter> GetParametersComponent()
    {
        return _FormLayout.Items.Single(x => x.Id == SelectedId).Parameters;
    }

    public void NewForm()
    {
        _FormLayout = new FormLayout
        {
            Id = Guid.NewGuid(),
            Name = "Demo",
            Description = "Demo",
            Rows = 0,
            Items = new List<FormComponent>(),
            State = FormState.Design
        };

        AddRow();
    }

    public async Task LoadForm(string idForm, FormState state)
    {
        var response = await _FormApiService.GetForm(idForm);

        if (response == null) return;

        _FormLayout = response;

        RestoreForm(state);
    }

    public async Task SaveForm()
    {
        FormLayout cleanTemplate = _FormLayout.Clone();
        cleanTemplate.Items.RemoveAll(x => x.State != FieldState.Hold);

        await _FormApiService.PostForm(cleanTemplate);
    }

    public void RestoreForm(FormState state)
    {
        int ixItem = 0;
        int joins = 0;

        IsBusy = true;

        //FormDesignerData.Items.Where(x => x.State == FieldState.Hold).ToList()
        //.ForEach(x => x.ComponentType = Type.GetType($"{x.TypeName}"));

        _FormLayout.State = FormState.Busy;
        //OnPropertyChanged(nameof(_FormLayout.State));

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
                        Parameters = new List<ComponentParameter>(),
                        Position = FieldPosition.MediumCenter,
                        State = joins > 1 ? FieldState.Disabled : FieldState.Empty
                    });

                    joins--;
                }
            }
        }

        _FormLayout.State = state;
        OnPropertyChanged(nameof(_FormLayout.Items));
        IsBusy = false;
    }

    public async Task<List<FormHistory>?> LoadHistory()
    {
        return await _FormApiService.GetHistory();
    }

    private async Task SaveHistory(Guid id, string name, string description)
    {
        await _FormApiService.PostHistory(id, name, description);
    }

    private object GetObjectByType(string typeName, string value)
    {
        object result = Convert.ChangeType(value, Type.GetType($"{typeName}"));
        return result;
    }

    public IDictionary<string, object> GetComponentParameters(List<ComponentParameter> componentParameters)
    {
        IDictionary<string, object> parametersDictionary = new Dictionary<string, object>();

        componentParameters.ForEach(x =>
        {
            parametersDictionary.Add(
                x.Name,
                GetObjectByType(x.TypeName, x.Value.ToString())
            );
        });

        return parametersDictionary;

        //foreach (var parameter in item.Parameters)
        //{
        //    string key = parameter.Key;
        //    JsonElement jsonElement = (JsonElement)parameter.Value;

        //    if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(jsonElement.GetString()))
        //    {
        //        item.Parameters.Remove(key);
        //        continue;
        //    }

        //    object? newParameterType = key switch
        //    {
        //        "Visible" => jsonElement.GetBoolean(),
        //        "Checked" => jsonElement.GetBoolean(),
        //        _ => jsonElement.GetString()
        //    };

        //    if (newParameterType == null)
        //    {
        //        item.Parameters.Remove(key);
        //        continue;
        //    }

        //    item.Parameters[key] = newParameterType;
        //}
    }

    private List<ComponentParameter> GetMockParameters()
    {
        
        return new List<ComponentParameter>() {
            new ComponentParameter { Name="Id", TypeName="System.String", Value="IdDemo" },
            new ComponentParameter { Name="Label", TypeName="System.String", Value="LabelDemo" },
            new ComponentParameter { Name="Checked", TypeName="System.Boolean", Value=false },
            //new ComponentParameter { Name="Data", TypeName="System.Text.Json", Value="{'Test'': 'Test''}" },
        };
    }

    #endregion

}