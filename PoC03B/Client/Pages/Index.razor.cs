namespace PoC03B.Client.Pages;

using Microsoft.AspNetCore.Components;
using MudBlazor;
using PoC03B.Shared;

public partial class Index
{
    [Inject] ISnackbar Snackbar { get; set; }

    [CascadingParameter(Name = "FormDesignerData")] protected FormDesignerModel FormDesignerData { get; set; }

    List<FormComponentModel> GetFormComponentsByRow(int rowId) => FormDesignerData.Items.Where(x => x.RowId == rowId && x.Xs != 0).ToList();

    int MainMode;
    string MainOperation = "MainMode";
    string dropClass = "";

    private void OnDragStart(string operation, Guid id)
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

        if (FormDesignerData.DragByTypeName != null)
        {
            targetComponent.TypeName = FormDesignerData.DragByTypeName;
            targetComponent.ComponentType = Type.GetType($"{FormDesignerData.DragByTypeName}");
            FormDesignerData.DragByTypeName = null;
        }

        if (FormDesignerData.DragByID != null)
        {
            var originComponent = FormDesignerData.Items.Single(x => x.Id == FormDesignerData.DragByID);

            switch (MainOperation)
            {
                case "MainMode":
                    Type? backupComponentType = targetComponent.ComponentType;
                    string? backupTypeName = targetComponent.TypeName;

                    targetComponent.ComponentType = originComponent.ComponentType;
                    targetComponent.TypeName = originComponent.TypeName;

                    originComponent.ComponentType = backupComponentType;
                    originComponent.TypeName = backupTypeName;

                    FormDesignerData.DragByID = null;
                    break;

                case "ResizeRight":
                    targetComponent.Xs = 0;
                    originComponent.Xs++;
                    break;
            }
        }
    }

    private void OnClick_MainMode(Guid id)
    {
        switch (MainMode)
        {
            case 0:

                break;

            case 1:
                FormDesignerData.Items.Remove(FormDesignerData.Items.Single(x => x.Id == id));
                break;
        }

        MainMode = 0;
    }

    private void ChangeMainMode()
    {
        MainMode++;

        if (MainMode > 1) MainMode = 0;
    }

    private void ResetMainMode()
    {
        MainMode = 0;
    }

    private string GetMainModeColor()
    {
        return MainMode switch
        {
            0 => "mud-theme-info",
            1 => "mud-theme-error",
        };
    }
}