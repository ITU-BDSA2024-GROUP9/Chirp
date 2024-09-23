using Chirp.CSVDBService.Interfaces;
using Chirp.CSVDBService.Classes;
using SimpleDB.Services;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// List<Cheep> x = new List<Cheep>();
// x?.Add(new Cheep("me1", "Hej!", 1684219348));
// x?.Add(new Cheep("me", "Hej!222", 1684229348));
// x?.Add(new Cheep("me2", "Hejzs!", 1684223348));

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

