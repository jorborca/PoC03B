namespace PoC03B.Client.Pages;

using Microsoft.AspNetCore.Components;
using MudBlazor;
using PoC03B.Shared.Enums;
using PoC03B.Shared.Models;

public partial class Index
{
    [Inject] ISnackbar Snackbar { get; set; }

    [CascadingParameter(Name = "FormDesignerData")] protected FormDesignerModel FormDesignerData { get; set; }

    List<FormComponentModel> GetFormComponentsByRow(int rowId) => FormDesignerData.Items.Where(x => x.RowId == rowId && x.State != FieldState.Disabled).ToList();

    FieldOperation MainOperation = FieldOperation.Move;
    string dropClass = "";

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
        if (MainOperation > FieldOperation.Expand) MainOperation = 0;
    }

    private void ProcessOperation(FormComponentModel originComponent, FormComponentModel targetComponent)
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
                List<FormComponentModel> itemsToSplit = FormDesignerData.Items.Where(
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
                List<FormComponentModel> itemsToJoin = FormDesignerData.Items.Where(
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
                    new FormComponentModel { 
                        ColId = 12
                    }
                );
                
                List<FormComponentModel> itemsToExpand = FormDesignerData.Items.Where(
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
        }
    }

    private void ResetMainOperation()
    {
        MainOperation = FieldOperation.Move;
    }

    private string GetMainOperationColor()
    {
        return MainOperation switch
        {
            FieldOperation.Move => "mud-theme-info",
            FieldOperation.Resize => "mud-theme-info",
            FieldOperation.Delete => "mud-theme-error",
            FieldOperation.Expand => "mud-theme-warning",
        };
    }

    private string GetPositionClass(FieldPosition position)
    {
        string horizontalPosition = string.Empty;
        string verticalPosition = string.Empty;

        switch (position)
        {
            case FieldPosition.TopCenter:
                verticalPosition = "align-items-top";
                horizontalPosition = "mud-typography-align-center";
                break;

            case FieldPosition.TopLeft:
                verticalPosition = "align-items-top";
                horizontalPosition = "mud-typography-align-left";
                break;

            case FieldPosition.TopRight:
                verticalPosition = "align-items-top";
                horizontalPosition = "mud-typography-align-right";
                break;

            case FieldPosition.CenterCenter:
                verticalPosition = "align-items-center";
                horizontalPosition = "mud-typography-align-center";
                break;

            case FieldPosition.BottomCenter:
                verticalPosition = "align-items-bottom";
                horizontalPosition = "mud-typography-align-center";
                break;

            case FieldPosition.BottomLeft:
                verticalPosition = "align-items-bottom";
                horizontalPosition = "mud-typography-align-left";
                break;

            case FieldPosition.BottomRight:
                verticalPosition = "align-items-bottom";
                horizontalPosition = "mud-typography-align-right";
                break;
        }

        return $"{horizontalPosition} {verticalPosition}";
    }
}