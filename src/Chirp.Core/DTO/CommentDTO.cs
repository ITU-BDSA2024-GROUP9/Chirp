using Chirp.Core.Classes;

namespace Chirp.Core.DTO;

public class CommentDTO
{
	public int CommentId { get; set; }
	public required string Text { get; set; }
	public required DateTime TimeStamp { get; set; }
	public required Author Author { get; set; }
	public required int CheepId { get; set; }
}