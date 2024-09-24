using Chirp.CSVDBService.Interfaces;
using Chirp.Core.Classes;
using SimpleDB.Services;
using System.Diagnostics;
using Chirp.CSVDBService;

var builder = WebApplication.CreateBuilder(args);

// Dependency injection using built-in ASP.NET Core framework
builder.Services.AddSingleton<NewCSVDatabaseService<Cheep>>();

var app = builder.Build();

NewCSVDatabaseService<Cheep> Database = app.Services.GetRequiredService<NewCSVDatabaseService<Cheep>>();

app.MapGet("/cheeps", async () => await Database.ReadAsync());
app.MapPost("/cheep", async (Cheep cheep) => await Database.StoreAsync(cheep));

app.Run();

