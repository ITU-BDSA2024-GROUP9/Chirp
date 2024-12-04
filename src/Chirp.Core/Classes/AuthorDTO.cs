using Microsoft.AspNetCore.Identity;

namespace Chirp.Core.Classes;

public class AuthorDTO : IdentityUser
{
    public required ICollection<Cheep> Cheeps { get; set; }
	
    public ICollection<Follow> Following { get; set; } = [];
    public ICollection<Follow> Followers { get; set; } = [];
    public ICollection<Comment> Comments { get; set; } = [];
}