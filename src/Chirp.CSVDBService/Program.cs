using Chirp.CSVDBService.Interfaces;
using Chirp.Core.Classes;
using SimpleDB.Services;
using System.Diagnostics;
using Chirp.CSVDBService;

var builder = WebApplication.CreateBuilder(args);

// Dependency injection using built-in ASP.NET Core framework
builder.Services.AddSingleton<CSVDatabaseService<Cheep>>();

var app = builder.Build();

var Database = app.Services.GetRequiredService<CSVDatabaseService<Cheep>>();

await Database.ArrangeTMPDatabase();

app.MapGet("/cheeps", async () => await Database.Read());
app.MapPost("/cheep", async (Cheep cheep) => await Database.Store(cheep));

await app.RunAsync();

