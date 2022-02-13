using PoC03B.Shared.Models;
using System.Net;
using System.Net.Http.Json;

namespace PoC03B.Client.Services;

public class FormApiService : IFormApiService
{
    private HttpClient _HttpClient { get; }

    public FormApiService(HttpClient httpClient)
    {
        this._HttpClient = httpClient;
    }

    public async Task<FormLayout> GetForm(string idForm)
    {
        var response = await _HttpClient.GetFromJsonAsync<FormLayout>($"templates/load/{idForm}");

        return response;
    }

    public async Task PostForm(FormLayout cleanTemplate)
    {
        await _HttpClient.PostAsJsonAsync("templates/save", cleanTemplate);
        await PostHistory(cleanTemplate.Id, cleanTemplate.Name, cleanTemplate.Description);
    }

    public async Task<List<FormHistory>?> GetHistory()
    {
        var response = await _HttpClient.GetAsync("history/load");

        if (response.StatusCode != HttpStatusCode.OK)
        {
            return null;
        }

        var formHistory = await response.Content.ReadFromJsonAsync<List<FormHistory>>();

        return formHistory;
    }

    public async Task PostHistory(Guid id, string name, string description)
    {
        bool Adding = false;
        List<FormHistory> formHistory = new();
        var response = await _HttpClient.GetAsync("history/load");

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

        await _HttpClient.PostAsJsonAsync("history/save", formHistory);
    }

}