var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

List<Cheep> x = new List<Cheep>();
x?.Add(new Cheep("me1", "Hej!", 1684219348));
x?.Add(new Cheep("me", "Hej!222", 1684229348));
x?.Add(new Cheep("me2", "Hejzs!", 1684223348));

List<Cheep> get()
{
    return x;
}


app.MapGet("/cheeps", () => get());
app.MapPost("/cheep", (Cheep cheep) =>
{
    x.Add(cheep);
    Console.WriteLine("recieved: " + cheep.Message); 
});

app.Run();

public record Cheep(string Author, string Message, long Timestamp);
