using Chirp.Core.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Core.Helpers;

public class ChirpDBContext : IdentityDbContext<Author>
{
	public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
	{
	}
	
	public DbSet<Cheep> Cheeps { get; set; }
	public DbSet<Author> Authors { get; set; }
	public DbSet<Follow> Follows { get; set; }
	
	
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// Configure entity relationships and other configurations here
		modelBuilder.Entity<Cheep>()
			.HasOne(c => c.Author)
			.WithMany(a => a.Cheeps)
			.HasForeignKey(c => c.AuthorId);

		// Enforce string length constraint on the Text property
		modelBuilder.Entity<Cheep>()
			.Property(c => c.Text)
			.HasMaxLength(160)
			.IsRequired();
		
		// Setting the composite key for the follows table
		modelBuilder.Entity<Follow>()
			.HasKey(f => new { f.FollowerId, f.FollowedId });
		
		modelBuilder.Entity<Follow>()
			.HasOne(f => f.Follower) // The `Follower` navigation property in `Follow`.
			.WithMany(a => a.Following) // The `Following` collection in the `Author` class.
			.HasForeignKey(f => f.FollowerId); // `FollowerId` is the foreign key in `Follow`.
	}
}