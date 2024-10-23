using System.ComponentModel.DataAnnotations;

namespace Chirp.Core.Classes;
public class Cheep
{
    public int CheepId { get; set; }
    [Required]
    [StringLength(160)]
    public required string Text { get; set; }
    public required DateTime TimeStamp { get; set; }
    public required Author Author { get; set; }
    public required string AuthorId { get; set; }
}