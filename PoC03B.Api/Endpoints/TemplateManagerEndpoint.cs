using PoC03B.Shared;
using System.Text.Json;

namespace PoC03B.Api.Endpoints
{
    public class TemplateManagerEndpoint
    {
        public async Task<IResult> SaveTemplateAsync(FormDesignerModel template)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", $"{template.Name}.json");
            string jsonTemplate = JsonSerializer.Serialize(template); 
            await File.WriteAllTextAsync(filePath, jsonTemplate);

            //using FileStream createStream = File.Create(fileName);
            //await JsonSerializer.SerializeAsync(createStream, FormDesignerData.Items);
            //await createStream.DisposeAsync();

            return Results.Ok();
        }

        public async Task<IResult> LoadTemplateAsync(string templateName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", $"{templateName}.json");
            string jsonTemplate = await File.ReadAllTextAsync(filePath);
            
            var template = JsonSerializer.Deserialize<FormDesignerModel>(jsonTemplate);

            if (template == null) return Results.NotFound();

            return Results.Ok(template);
        }

    }
}
