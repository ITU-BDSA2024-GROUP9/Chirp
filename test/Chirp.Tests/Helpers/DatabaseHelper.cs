using Chirp.Core.Classes;
using Chirp.Repositories.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Tests.Helpers;

// ref: https://learn.microsoft.com/en-us/ef/core/testing/testing-with-the-database 
public class DatabaseHelper : IDisposable
{
	public readonly SqliteConnection ConnectionString;
	public readonly List<Author> Authors;

	public DatabaseHelper(string connection)
	{
		// Create an in-memory SQLite connection
		ConnectionString = new SqliteConnection(connection);
		ConnectionString.Open();

		var options = new DbContextOptionsBuilder<ChirpDBContext>()
			.UseSqlite(ConnectionString)
			.Options;

		var context = new ChirpDBContext(options);

		// Ensure the schema is created, including Identity tables (via migrations)
		context.Database.EnsureCreated();  // This will create tables from any migrations applied

		// Optionally, apply pending migrations if necessary
		context.Database.Migrate();  // Apply any pending migrations to ensure all tables (including Identity) are created
		
		// Seed the database with initial data, if necessary
		Authors = DbInitializer.SeedDatabase(context);
		
		// TODO - run dbinitializer.setpasswords() somehow
	}

	public ChirpDBContext CreateContext()
	{
		var options = new DbContextOptionsBuilder<ChirpDBContext>()
			.UseSqlite(ConnectionString)
			.Options;

		return new ChirpDBContext(options);
	}

	public void Dispose()
	{
		ConnectionString.Dispose();  // Cleanup
	}
}

