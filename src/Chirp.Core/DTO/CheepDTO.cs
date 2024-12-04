using Chirp.Core.Classes;
using System.ComponentModel.DataAnnotations;

namespace Chirp.Core.DTO;

public class CheepDTO
{
	public int CheepId { get; set; }
	[Required]
	[StringLength(160)]
	public required string Text { get; set; }
	public required DateTime TimeStamp { get; set; }
	public required Author Author { get; set; }
	public List<string>? Images { get; set; }
}