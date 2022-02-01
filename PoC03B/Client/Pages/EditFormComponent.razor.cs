namespace PoC03B.Client.Pages;

using Microsoft.AspNetCore.Components;
using MudBlazor;
using PoC03B.Shared.Enums;
using PoC03B.Shared.Models;
using System.Net.Http.Json;
using System.Threading.Tasks;

public partial class EditFormComponent
{
    [Inject] HttpClient Http { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }

    [CascadingParameter(Name = "FormDesignerData")]
    protected FormDesigner FormDesignerData { get; set; }

    [Parameter]
    public string? idTemplate { get; set; }

    List<FormComponent> GetFormComponentsByRow(int rowId) => FormDesignerData.Items.Where(x => x.RowId == rowId && x.State != FieldState.Disabled).ToList();
    FieldOperation MainOperation = FieldOperation.Move;
    string dropClass = "";

    protected override async Task OnInitializedAsync()
    {
        if (idTemplate != null)
        {
            await LoadTemplate(idTemplate);
        }
    }

    private void OnDragStart(FieldOperation operation, Guid id)
    {
        MainOperation = operation;
        FormDesignerData.DragByID = id;
    }

    private void OnDragEnter(Guid id)
    {
        //if (AllowedStatuses != null && !AllowedStatuses.Contains(Container.Payload.Status))
        //{
        //    dropClass = "no-drop";
        //}
        //else
        //{
        //    dropClass = "can-drop";
        //}
    }

    private void OnDragOver()
    {

    }

    private void OnDragLeave()
    {
        //dropClass = "";
    }

    private void OnDrop(Guid id)
    {
        var targetComponent = FormDesignerData.Items.Single(x => x.Id == id);

        // Drag Item from Toolbar to Field
        if (FormDesignerData.DragByTypeName != null)
        {
            targetComponent.TypeName = FormDesignerData.DragByTypeName;
            targetComponent.ComponentType = Type.GetType($"{FormDesignerData.DragByTypeName}");
            targetComponent.State = FieldState.Hold;
            FormDesignerData.DragByTypeName = null;
        }

        // Drag Item between Fields
        if (FormDesignerData.DragByID != null)
        {
            var originComponent = FormDesignerData.Items.Single(x => x.Id == FormDesignerData.DragByID);
            ProcessOperation(originComponent, targetComponent);
            FormDesignerData.DragByID = null;
        }
    }

    private void OnClick_MainOperation(Guid id)
    {
        var originComponent = FormDesignerData.Items.Single(x => x.Id == id);
        ProcessOperation(originComponent, originComponent);
        MainOperation = FieldOperation.Move;
    }

    private void OnChange_MainOperation()
    {
        MainOperation++;
        if (MainOperation > FieldOperation.Split) MainOperation = 0;
    }

    private void OnMouseLeave_MainOperation()
    {
        MainOperation = FieldOperation.Move;
    }

    private void ProcessOperation(FormComponent originComponent, FormComponent targetComponent)
    {
        switch (MainOperation)
        {
            case FieldOperation.Move:
                Type? backupComponentType = targetComponent.ComponentType;
                string? backupTypeName = targetComponent.TypeName;

                targetComponent.ComponentType = originComponent.ComponentType;
                targetComponent.TypeName = originComponent.TypeName;
                targetComponent.State = FieldState.Hold;

                // Restore and Split the collapsed fields
                List<FormComponent> itemsToSplit = FormDesignerData.Items.Where(
                    x => x.RowId == originComponent.RowId
                    && x.State == FieldState.Disabled
                    && x.ColId > originComponent.ColId
                    && x.ColId <= originComponent.ColId + originComponent.Xs
                ).ToList();

                itemsToSplit.ForEach(x => {
                    x.State = FieldState.Empty;
                });

                originComponent.ComponentType = backupComponentType;
                originComponent.TypeName = backupTypeName;
                originComponent.State = FieldState.Empty;
                originComponent.Xs = 1;
                break;

            case FieldOperation.Resize:
                //Snackbar.Add($"Origin: {originComponent.ColId}-{originComponent.Xs}");
                //Snackbar.Add($"Target: {targetComponent.ColId}-{targetComponent.Xs}");

                // Join fields
                List<FormComponent> itemsToJoin = FormDesignerData.Items.Where(
                    x => x.RowId == originComponent.RowId
                    && x.State == FieldState.Empty
                    && x.ColId > originComponent.ColId
                    && x.ColId <= targetComponent.ColId
                ).ToList();

                itemsToJoin.ForEach(x => {
                    x.State = FieldState.Disabled;
                    originComponent.Xs++;
                });
                break;

            case FieldOperation.Delete: // Eliminar Item
                FormDesignerData.Items.Remove(originComponent);
                break;

            case FieldOperation.Expand:
                var limitComponent = FormDesignerData.Items.FirstOrDefault(
                    x => x.RowId == originComponent.RowId
                    && x.ColId > originComponent.ColId
                    && x.State == FieldState.Hold,
                    new FormComponent
                    {
                        ColId = 12
                    }
                );

                List<FormComponent> itemsToExpand = FormDesignerData.Items.Where(
                    x => x.RowId == originComponent.RowId
                    && x.ColId > originComponent.ColId
                    && x.ColId <= limitComponent.ColId
                    && x.State == FieldState.Empty
                ).ToList();

                itemsToExpand.ForEach(x => {
                    x.State = FieldState.Disabled;
                    originComponent.Xs++;
                });
                break;

            case FieldOperation.Split:
                if (originComponent.Xs > 1)
                {
                    FormDesignerData.Items.Where(
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

    private string GetMainOperationColor()
    {
        return MainOperation switch
        {
            FieldOperation.Move => "mud-theme-info",
            FieldOperation.Resize => "mud-theme-info",
            FieldOperation.Delete => "mud-theme-error",
            FieldOperation.Expand => "mud-theme-success",
            FieldOperation.Split => "mud-theme-dark",
        };
    }

    private string GetPositionClass(FieldPosition position)
    {
        string verticalPosition = "align-items-top";
        string horizontalPosition = "mud-typography-align-left";

        if (position.ToString().Contains("Top")) verticalPosition = "align-items-top";
        if (position.ToString().Contains("Medium")) verticalPosition = "align-items-center";
        if (position.ToString().Contains("Bottom")) verticalPosition = "align-items-bottom";

        if (position.ToString().Contains("Left")) horizontalPosition = "mud-typography-align-left";
        if (position.ToString().Contains("Center")) horizontalPosition = "mud-typography-align-center";
        if (position.ToString().Contains("Right")) horizontalPosition = "mud-typography-align-right";

        return $"{horizontalPosition} {verticalPosition}";
    }

    private async Task LoadTemplate(string id)
    {
        var response = await Http.GetFromJsonAsync<FormDesigner>($"templates/load/{id}");
        if (response == null) return;
        FormDesignerData = response;
        RestoreForm();
    }

    private void RestoreForm()
    {
        int ixItem = 0;
        int joins = 0;

        //FormDesignerData.Items.Where(x => x.State == FieldState.Hold).ToList()
        //    .ForEach(x => x.ComponentType = Type.GetType($"{x.TypeName}"));

        for (int rowId = 1; rowId <= FormDesignerData.Rows; rowId++)
        {
            for (int colId = 1; colId <= 12; colId++)
            {
                if (FormDesignerData.Items.Any(x => x.RowId == rowId && x.ColId == colId))
                {
                    var item = FormDesignerData.Items.Single(x => x.RowId == rowId && x.ColId == colId);
                    item.ComponentType = Type.GetType($"{item.TypeName}");

                    if (item.Xs > 1) joins = item.Xs;
                }
                else
                {
                    ixItem = ((rowId - 1) * 12) + (colId - 1);

                    FormDesignerData.Items.Insert(ixItem, new FormComponent()
                    {
                        Id = Guid.NewGuid(),
                        RowId = rowId,
                        ColId = colId,
                        Xs = 1,
                        Sm = 1,
                        Md = 1,
                        Lg = 1,
                        Position = FieldPosition.MediumCenter,
                        State = (joins > 1) ? FieldState.Disabled : FieldState.Empty
                    });

                    joins--;
                }
            }
        }

    }

}