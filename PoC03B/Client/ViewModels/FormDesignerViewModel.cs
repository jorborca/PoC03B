namespace PoC03B.Client.ViewModels;

using PoC03B.Client.Services;
using PoC03B.Shared.Enums;
using PoC03B.Shared.Models;

public class FormDesignerViewModel : BaseViewModel, IFormDesignerViewModel
{
    private FormDesigner _FormDesignerData { get; set; }
    private IFormDesignerService _FormDesignerService { get; set; }

    public FormDesignerViewModel(IFormDesignerService formDesignerService)
    {
        this._FormDesignerData = new();
        this._FormDesignerService = formDesignerService;
    }

    public void AddRow()
    {
        _FormDesignerData.Rows++;

        if (!_FormDesignerData.Items.Any(x => x.RowId == _FormDesignerData.Rows))
        {
            for (int colId = 1; colId <= 12; colId++)
            {
                _FormDesignerData.Items.Add(new FormComponent()
                {
                    Id = Guid.NewGuid(),
                    RowId = _FormDesignerData.Rows,
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
    }

    public void RemoveRow()
    {
        if (_FormDesignerData.Rows == 1) return;

        int rowId = _FormDesignerData.Rows;

        if (!_FormDesignerData.Items.Any(x => x.RowId == rowId && x.State == FieldState.Hold))
        {
            _FormDesignerData.Items.RemoveAll(x => x.RowId == rowId);
            _FormDesignerData.Rows--;
        }
    }

    public int GetRowsCount()
    {
        return _FormDesignerData.Rows;
    }


    public void SetState(FormState state)
    {
        _FormDesignerData.State = state;
    }

    public FormState GetState()
    {
        return _FormDesignerData.State;
    }

    public bool CheckState(FormState state)
    {
        return _FormDesignerData.State == state;
    }


    public void SetDragTypeName(string typeName)
    {
        _FormDesignerData.DragByTypeName = typeName;
    }

    public void SetDragID(Guid id)
    {
        _FormDesignerData.DragByID = id;
    }


    public List<FormComponent> GetFormComponentsByRow(int rowId)
    {
        return _FormDesignerData.Items.Where(x => x.RowId == rowId && x.State != FieldState.Disabled).ToList();
    }

    public void ProcessOperation(FieldOperation mainOperation, Guid? idOriginComponent, Guid? idTargetComponent)
    {
        FormComponent originComponent = new();
        FormComponent targetComponent = new();

        if (_FormDesignerData.DragByID != null)
        {
            idOriginComponent = _FormDesignerData.DragByID;
            _FormDesignerData.DragByID = null;
        }

        if (idOriginComponent != null)
        {
            originComponent = _FormDesignerData.Items.Single(x => x.Id == idOriginComponent);
        }

        if (idTargetComponent != null)
        {
            targetComponent = _FormDesignerData.Items.Single(x => x.Id == idTargetComponent);
        }

        switch (mainOperation)
        {
            case FieldOperation.Move:
                Type? backupComponentType = targetComponent.ComponentType;
                string? backupTypeName = targetComponent.TypeName;

                if (_FormDesignerData.DragByTypeName != null)
                {
                    targetComponent.TypeName = _FormDesignerData.DragByTypeName;
                    targetComponent.ComponentType = Type.GetType($"{_FormDesignerData.DragByTypeName}");
                    targetComponent.State = FieldState.Hold;
                    _FormDesignerData.DragByTypeName = null;
                }
                else
                {
                    targetComponent.TypeName = originComponent.TypeName;
                    targetComponent.ComponentType = originComponent.ComponentType;
                    targetComponent.State = FieldState.Hold;

                    RestoreField(originComponent, backupComponentType, backupTypeName);
                }
                break;

            case FieldOperation.Resize:
                //Snackbar.Add($"Origin: {originComponent.ColId}-{originComponent.Xs}");
                //Snackbar.Add($"Target: {targetComponent.ColId}-{targetComponent.Xs}");

                // Join fields
                List<FormComponent> itemsToJoin = _FormDesignerData.Items.Where(
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
                RestoreField(originComponent, null, null);
                break;

            case FieldOperation.Expand:
                var limitComponent = _FormDesignerData.Items.FirstOrDefault(
                    x => x.RowId == originComponent.RowId
                    && x.ColId > originComponent.ColId
                    && x.State == FieldState.Hold,
                    new FormComponent
                    {
                        ColId = 12
                    }
                );

                List<FormComponent> itemsToExpand = _FormDesignerData.Items.Where(
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
                    _FormDesignerData.Items.Where(
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

    private void RestoreField(FormComponent originComponent, Type? newComponentType, string? newTypeName)
    {
        // Restore and Split the collapsed fields
        List<FormComponent> itemsToSplit = _FormDesignerData.Items.Where(
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
        originComponent.State = FieldState.Empty;
        originComponent.Xs = 1;
    }

    public void NewForm()
    {
        _FormDesignerData = new FormDesigner
        {
            Id = Guid.NewGuid(),
            Name = "Demo",
            Description = "Demo",
            Rows = 0,
            State = FormState.Design
        };

        AddRow();
    }

    public async Task LoadForm(string idForm)
    {
        int ixItem = 0;
        int joins = 0;

        var response = await _FormDesignerService.GetForm(idForm);

        if (response == null) return;

        _FormDesignerData = response;

        //FormDesignerData.Items.Where(x => x.State == FieldState.Hold).ToList()
        //.ForEach(x => x.ComponentType = Type.GetType($"{x.TypeName}"));

        for (int rowId = 1; rowId <= _FormDesignerData.Rows; rowId++)
        {
            for (int colId = 1; colId <= 12; colId++)
            {
                if (_FormDesignerData.Items.Any(x => x.RowId == rowId && x.ColId == colId))
                {
                    var item = _FormDesignerData.Items.Single(x => x.RowId == rowId && x.ColId == colId);
                    item.ComponentType = Type.GetType($"{item.TypeName}");

                    if (item.Xs > 1) joins = item.Xs;
                }
                else
                {
                    ixItem = (rowId - 1) * 12 + (colId - 1);

                    _FormDesignerData.Items.Insert(ixItem, new FormComponent()
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

    public async Task SaveForm()
    {
        FormDesigner cleanTemplate = _FormDesignerData.Clone();
        cleanTemplate.Items.RemoveAll(x => x.State != FieldState.Hold);

        await _FormDesignerService.PostForm(cleanTemplate);
    }


    public async Task<List<FormHistory>?> LoadHistory()
    {
        return await _FormDesignerService.GetHistory();
    }

    private async Task SaveHistory(Guid id, string name, string description)
    {
        await _FormDesignerService.PostHistory(id, name, description);
    }
}