using Microsoft.AspNetCore.Mvc;
using PoC03B.Api.Endpoints;
using PoC03B.Shared;
//using PoC03B.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddSingleton(new WeatherService());
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllOrigins",
        builder =>
        {
            builder.AllowAnyHeader()
            .AllowAnyOrigin()
            .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllOrigins");

//app.MapGet("/weatherforecast", (HttpContext _, WeatherService weatherService) => weatherService.GetWeatherForecasts())
//.WithName("GetWeatherForecast");

//var weatherEndpoint = new WeatherEndpoint();
//app.MapGet("/weatherforecast", weatherEndpoint.GetWeatherForecasts).WithName("GetWeatherForecast");

var templateManagerEndpoint = new TemplateManagerEndpoint();
app.MapGet("/templates/load/{templateName}", (string templateName) => templateManagerEndpoint.LoadTemplateAsync(templateName)).WithName("LoadTemplate");
app.MapPost("/templates/save", ([FromBody] FormDesignerModel template) => templateManagerEndpoint.SaveTemplateAsync(template)).WithName("SaveTemplate");

app.Run();