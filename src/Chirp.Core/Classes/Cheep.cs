using System.ComponentModel.DataAnnotations;

namespace Chirp.Core.Classes;
/// <summary>
/// Represents a Cheep and contains key information about a given cheep, such as its unnique ID, text, datestamp, etc.
/// </summary>
public class Cheep
{
	public int CheepId { get; set; }
	[Required]
	[StringLength(160)]
	public required string Text { get; set; }
	public required DateTime TimeStamp { get; set; }
	public required Author Author { get; set; }
	public required string AuthorId { get; set; }
	public List<string>? Images { get; set; }
}