var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


List<Cheep> get()
{
    var x = new List<Cheep>();
    x.Add(new Cheep("me", "Hej!", 1684229348));
    return x;
}


app.MapGet("/cheeps", () => get());
app.MapPost("/cheep", (Cheep cheep) => { Console.WriteLine("recieved: " + cheep.Message); });

app.Run();

public record Cheep(string Author, string Message, long Timestamp);
