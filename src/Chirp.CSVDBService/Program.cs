using Chirp.CSVDBService.Interfaces;
using Chirp.Core.Classes;
using SimpleDB.Services;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

List<Cheep> get()
{
    try
    {
        var csv = CSVDatabaseService<Cheep>.Instance;
        return csv.Read().Result;
    } catch (Exception e) 
    {
        System.Diagnostics.Trace.TraceInformation(e.Message);
        return [new Cheep("There was an error!", "sad face", 0)];
    }

}

app.MapGet("/cheeps", () => get());
app.MapPost("/cheep", async (Cheep cheep) =>
{
    var csv = CSVDatabaseService<Cheep>.Instance;
    await csv.Store(cheep);
    Console.WriteLine("recieved: " + cheep.Message); 
});

app.Run();

