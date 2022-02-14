namespace PoC03B.Client.ViewModels;

using PoC03B.Client.Services;
using PoC03B.Shared.Enums;
using PoC03B.Shared.Models;
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

    public string GetFormId()
    {
        return _FormLayout.Id.ToString();
    }

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

    public int GetRowsCount()
    {
        return _FormLayout.Rows;
    }


    public void SetState(FormState state)
    {
        //IsBusy = true;
        _FormLayout.State = state;
        //OnPropertyChanged(nameof(_FormLayout.State));
        //IsBusy = false;
    }

    public FormState GetState()
    {
        return _FormLayout.State;
    }

    public bool CheckState(FormState state)
    {
        return _FormLayout.State == state;
    }


    public void SetDragTypeName(string typeName)
    {
        _FormLayout.DragByTypeName = typeName;
    }

    public void SetDragID(Guid id)
    {
        _FormLayout.DragByID = id;
    }


    public List<FormComponent> GetFormComponentsByRow(int rowId)
    {
        return _FormLayout.Items.Where(x => x.RowId == rowId && x.State != FieldState.Disabled).ToList();
    }

    public void ProcessOperation(FieldOperation mainOperation, Guid? idOriginComponent, Guid? idTargetComponent)
    {
        FormComponent originComponent = new();
        FormComponent targetComponent = new();

        if (_FormLayout.DragByID != null)
        {
            idOriginComponent = _FormLayout.DragByID;
            _FormLayout.DragByID = null;
        }

        if (idOriginComponent != null)
        {
            originComponent = _FormLayout.Items.Single(x => x.Id == idOriginComponent);
        }

        if (idTargetComponent != null)
        {
            targetComponent = _FormLayout.Items.Single(x => x.Id == idTargetComponent);
        }

        switch (mainOperation)
        {
            case FieldOperation.Move:
                if (_FormLayout.DragByTypeName != null) // Drag from ToolBar
                {
                    targetComponent.TypeName = _FormLayout.DragByTypeName;
                    targetComponent.ComponentType = Type.GetType($"{_FormLayout.DragByTypeName}");
                    targetComponent.Parameters = new Dictionary<string, object>() {
                        { "Id", _FormLayout.DragByTypeName },
                        { "Label", "Label" }
                    };
                    targetComponent.State = FieldState.Hold;
                    _FormLayout.DragByTypeName = null;
                }
                else
                {
                    Type? backupComponentType = targetComponent.ComponentType;
                    string? backupTypeName = targetComponent.TypeName;
                    IDictionary<string, object> backupParameters = targetComponent.Parameters;

                    targetComponent.TypeName = originComponent.TypeName;
                    targetComponent.ComponentType = originComponent.ComponentType;
                    targetComponent.Parameters = originComponent.Parameters;
                    targetComponent.State = FieldState.Hold;

                    RestoreField(originComponent, backupComponentType, backupTypeName, backupParameters);
                }
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
                RestoreField(originComponent, null, null, new Dictionary<string, object>());
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
    }

    private void RestoreField(FormComponent originComponent, Type? newComponentType, string? newTypeName, IDictionary<string, object> parameters)
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

    public void NewForm()
    {
        _FormLayout = new FormLayout
        {
            Id = Guid.NewGuid(),
            Name = "Demo",
            Description = "Demo",
            Rows = 0,
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

                    if (item.Parameters.Any())
                    {
                        foreach (var parameter in item.Parameters)
                        {
                            string key = parameter.Key;
                            JsonElement jsonElement = (JsonElement)parameter.Value;

                            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(jsonElement.GetString()))
                            {
                                item.Parameters.Remove(key);
                                continue;
                            }

                            object? newParameterType = key switch
                            {
                                "Visible" => jsonElement.GetBoolean(),
                                _ => jsonElement.GetString()
                            };

                            if (newParameterType == null)
                            {
                                item.Parameters.Remove(key);
                                continue;
                            }

                            item.Parameters[key] = newParameterType;
                        }
                    }
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
                        Parameters = new Dictionary<string, object>(),
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
}