namespace Chirp.Core.Classes;

public class CheepDTO {
    public required string Text { get; set; }
    public required DateTime TimeStamp { get; set; }
    public required Author Author { get; set; }
    
}