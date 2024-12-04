namespace Chirp.Core.Classes;

public class Comment
{
    public int CommentId { get; set; }
    public required string Text { get; set; }
    public required DateTime TimeStamp { get; set; }
    public required Author Author { get; set; }
    public required string AuthorId { get; set; }
    public required Cheep Cheep { get; set; }
    public required int CheepId { get; set; }
}