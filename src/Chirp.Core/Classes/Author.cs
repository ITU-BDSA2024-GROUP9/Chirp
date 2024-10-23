
using Microsoft.AspNetCore.Identity;

namespace Chirp.Core.Classes;
public class Author : IdentityUser
{
	public required ICollection<Cheep> Cheeps { get; set; }
}