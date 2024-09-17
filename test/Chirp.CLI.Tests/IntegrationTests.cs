using Chirp.CLI;
using Chirp.Core.Classes;
using SimpleDB.Services;

namespace Chirp.CLI.Tests;

public class IntegrationTests
{
    [Fact]
    async public void TestRead()
    {
        // Arrange
        var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		var csvDB = CSVDatabaseService<Cheep>.Instance;
        await csvDB.ArrangeTestDatabase();
        // Act
        Cheep _cheep = new("a", "b", time);
		await csvDB.Store(_cheep);
    
        // Assert
		Assert.Equal(_cheep, (await csvDB.Read()).Last());

		// Act
		await csvDB.Delete(_cheep);

		// Assert
		Assert.DoesNotContain(_cheep, await csvDB.Read());
    }
}