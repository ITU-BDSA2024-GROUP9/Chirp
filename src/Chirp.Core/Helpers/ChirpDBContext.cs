using Chirp.Core.Classes;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Core.Helpers;

public class ChirpDBContext : DbContext
{
	public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
	{
	}
	
	public DbSet<Cheep> Cheeps { get; set; }
	public DbSet<Author> Authors { get; set; }
}