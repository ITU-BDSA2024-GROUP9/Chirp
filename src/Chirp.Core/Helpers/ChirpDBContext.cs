using Chirp.Core.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace Chirp.Core.Helpers;
public class ChirpDBContext : IdentityDbContext<Author>
{
	public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
	{
	}

	public DbSet<Cheep> Cheeps { get; set; }
	public DbSet<Author> Authors { get; set; }
	public DbSet<Follow> Follows { get; set; }
	public DbSet<Comment> Comments { get; set; }


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
			.HasOne(f => f.Follower)
			.WithMany(a => a.Following)
			.HasForeignKey(f => f.FollowerId);

		modelBuilder.Entity<Follow>()
			.HasOne(f => f.Followed)
			.WithMany(a => a.Followers)
			.HasForeignKey(f => f.FollowedId);

		modelBuilder.Entity<Comment>()
			.HasOne(c => c.Author)
			.WithMany(a => a.Comments)
			.HasForeignKey(c => c.AuthorId);


		var imagesConverter = new ValueConverter<List<string>?, string>(
			static v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
			static v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default) ?? new List<string>());

		_ = modelBuilder.Entity<Cheep>()
			.Property(c => c.Images)
			.HasConversion(imagesConverter);
	}
}