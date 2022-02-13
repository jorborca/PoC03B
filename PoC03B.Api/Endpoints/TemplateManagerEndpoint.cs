using PoC03B.Shared.Models;
using System.Text.Json;

namespace PoC03B.Api.Endpoints;

public class TemplateManagerEndpoint
{
    public async Task<IResult> SaveTemplateAsync(FormLayout template)
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", $"{template.Id}.json");
        string jsonTemplate = JsonSerializer.Serialize(template); 
        await File.WriteAllTextAsync(filePath, jsonTemplate);

        //using FileStream createStream = File.Create(fileName);
        //await JsonSerializer.SerializeAsync(createStream, FormDesignerData.Items);
        //await createStream.DisposeAsync();

        return Results.Ok();
    }

    public async Task<IResult> LoadTemplateAsync(string id)
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", $"{id}.json");
        string jsonTemplate = await File.ReadAllTextAsync(filePath);
            
        var template = JsonSerializer.Deserialize<FormLayout>(jsonTemplate);

        if (template == null) return Results.NotFound();

        return Results.Ok(template);
    }
}