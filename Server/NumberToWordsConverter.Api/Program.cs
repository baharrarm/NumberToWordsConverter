using NumberToWordsConverter.Api.Endpoints;
using NumberToWordsConverter.Application.Services;
using NumberToWordsConverter.Application.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<INumberConverterService, NumberConverterService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapNumberConverterEndpoints();


app.Run();

public partial class Program { }

