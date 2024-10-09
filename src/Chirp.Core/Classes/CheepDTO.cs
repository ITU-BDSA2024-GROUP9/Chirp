using System.ComponentModel.DataAnnotations;

namespace Chirp.Core.Classes;

public class CheepDTO {
    [Required]
    [StringLength(160)]
    public required string Text { get; set; }
    public required DateTime TimeStamp { get; set; }
    public required Author Author { get; set; }
    
}