using PoC03B.Shared.Enums;
using PoC03B.Shared.Models;
using System.Net;
using System.Net.Http.Json;

namespace PoC03B.Client.ViewModels
{
    public class FormDesignerViewModel
    {
        public FormDesigner FormDesignerData { get; set; }
        public HttpClient HttpClient { get; }

        public FormDesignerViewModel(HttpClient httpClient)
        {
            FormDesignerData = new();
            HttpClient = httpClient;
        }

        public void AddRow(int increment)
        {
            FormDesignerData.Rows += increment;

            if (!FormDesignerData.Items.Any(x => x.RowId == FormDesignerData.Rows))
            {
                for (int colId = 1; colId <= 12; colId++)
                {
                    FormDesignerData.Items.Add(new FormComponent()
                    {
                        Id = Guid.NewGuid(),
                        RowId = FormDesignerData.Rows,
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
            if (FormDesignerData.Rows == 1) return;

            int rowId = FormDesignerData.Rows;

            if (!FormDesignerData.Items.Any(x => x.RowId == rowId && x.State == FieldState.Hold))
            {
                FormDesignerData.Items.RemoveAll(x => x.RowId == rowId);
                FormDesignerData.Rows--;
            }
        }

        public int GetRowsCount()
        {
            return FormDesignerData.Rows;
        }


        public void SetState(FormState state)
        {
            FormDesignerData.State = state;
        }

        public FormState GetState()
        {
            return FormDesignerData.State;
        }

        public bool CheckState(FormState state)
        {
            return FormDesignerData.State == state;
        }

        public void SetDragTypeName(string typeName)
        {
            FormDesignerData.DragByTypeName = typeName;
        }


        public void NewForm()
        {
            FormDesignerData = new FormDesigner
            {
                Id = Guid.NewGuid(),
                Name = "Demo",
                Description = "Demo",
                Rows = 1,
                State = FormState.Design
            };

            AddRow(0);
        }

        public void LoadForm()
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
                        ixItem = (rowId - 1) * 12 + (colId - 1);

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
                            State = joins > 1 ? FieldState.Disabled : FieldState.Empty
                        });

                        joins--;
                    }
                }
            }
        }

        public async Task SaveForm()
        {
            FormDesigner cleanTemplate = FormDesignerData.Clone();

            cleanTemplate.Items.RemoveAll(x => x.State != FieldState.Hold);
            await HttpClient.PostAsJsonAsync("templates/save", cleanTemplate);

            await SaveHistory(cleanTemplate.Id, cleanTemplate.Name, cleanTemplate.Description);
        }


        public async Task<List<FormHistory>?> LoadHistory()
        {
            var response = await HttpClient.GetAsync("history/load");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var formHistory = await response.Content.ReadFromJsonAsync<List<FormHistory>>();

            return formHistory;
        }

        private async Task SaveHistory(Guid id, string name, string description)
        {
            bool Adding = false;
            List<FormHistory> formHistory = new();
            var response = await HttpClient.GetAsync("history/load");

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    formHistory = await response.Content.ReadFromJsonAsync<List<FormHistory>>();

                    if (formHistory.Any(x => x.Id == id))
                    {
                        var history = formHistory.First(x => x.Id == id);
                        history.SavedDate = DateTime.Now;
                    }
                    else
                    {
                        Adding = true;
                    }
                    break;

                case HttpStatusCode.NotFound:
                    Adding = true;
                    break;

                default:
                case HttpStatusCode.BadRequest:
                    return;
            }

            if (Adding)
            {
                formHistory.Add(new FormHistory()
                {
                    Id = id,
                    Name = name,
                    Description = description,
                    SavedDate = DateTime.Now
                });
            }

            await HttpClient.PostAsJsonAsync("history/save", formHistory);
        }

    }
}
