using PoC03B.Shared.Models;
using System.Text.Json;

namespace PoC03B.Api.Endpoints;

public class HistoryEndpoint
{
    public async Task<IResult> SaveHistoryAsync(List<FormHistory> history)
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "History.json");
        string jsonTemplate = JsonSerializer.Serialize(history);
        await File.WriteAllTextAsync(filePath, jsonTemplate);
        
        return Results.Ok();
    }

    public async Task<IResult> LoadHistoryAsync()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "History.json");
        string jsonHistory = await File.ReadAllTextAsync(filePath);

        var history = JsonSerializer.Deserialize<List<FormHistory>>(jsonHistory);

        if (history == null) return Results.NotFound();

        return Results.Ok(history);
    }
}