using Chirp.Core.Classes;
using Chirp.Core.Interfaces;
using SimpleDB.Services;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Dependency injection using built-in ASP.NET Core framework
builder.Services.AddScoped<IDatabaseRepository<Cheep>, CSVDatabaseService<Cheep>>();

var app = builder.Build();

var Database = app.Services.GetRequiredService<IDatabaseRepository<Cheep>>();

await Database.ArrangeTMPDatabase();

app.MapGet("/cheeps", async () => await Database.Read());
app.MapPost("/cheep", async (Cheep cheep) => await Database.Store(cheep));

await app.RunAsync();

