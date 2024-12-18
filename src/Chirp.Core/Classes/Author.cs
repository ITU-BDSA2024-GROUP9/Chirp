
using Microsoft.AspNetCore.Identity;

namespace Chirp.Core.Classes;
/// <summary>
/// Represents an Author within the application, inheriting from the IdentityUser class.
/// </summary>
public class Author : IdentityUser
{
	public required ICollection<Cheep> Cheeps { get; set; }

	public ICollection<Follow> Following { get; set; } = [];
	public ICollection<Follow> Followers { get; set; } = [];
	public ICollection<Comment> Comments { get; set; } = [];
}