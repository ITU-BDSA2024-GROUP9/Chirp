using System.ComponentModel.DataAnnotations;

namespace Chirp.Core.Classes;

public class CheepDTO {
    public int CheepId { get; set; }
    [Required]
    [StringLength(160)]
    public required string Text { get; set; }
    public required DateTime TimeStamp { get; set; }
    public required AuthorDTO Author { get; set; }
    public List<string>? Images { get; set; }
}