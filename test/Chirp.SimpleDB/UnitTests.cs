using Chirp.Core.Classes;
using SimpleDB.Services;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Chirp.SimpleDB;

public class UnitTests
{
    [Fact]
    async public void TestRead()
    {
        //Arrange
        var csv = CSVDatabaseService<Cheep>.Instance;

        //Act
        var it = await csv.Read(1);
        Cheep _cheep = new Cheep("ropf", "Hello, BDSA students!", 1690891760);

        //Assert
        Assert.Equal(_cheep, it.First());
    }

    [Fact]
    async public void TestDelete()
    {
        // Arrange
        var cheep = new Cheep("toremove", "toremove", 1690981487);
        var csv = CSVDatabaseService<Cheep>.Instance;
        await csv.ArrangeTestDatabase();

        // Act
        await csv.Store(cheep);
        await csv.Delete(cheep);
        var data = await csv.Read();

        // Assert
        Assert.DoesNotContain(cheep, data);
    }

    [Fact]
    async public void TestWrite()
    {
        // Arrange
        var cheep = new Cheep("a", "b", 1690981487);
        var csv = CSVDatabaseService<Cheep>.Instance;
        await csv.ArrangeTestDatabase();

        // Act
        await csv.Store(cheep);
        var data = await csv.Read();
        Cheep lastCheep = data.Last();

        // Assert
        Assert.Equal(cheep, lastCheep);
    }

    [Fact]
    async public void TestArrangeTestDatabase()
    {
        // Arrange
        var csv = CSVDatabaseService<Cheep>.Instance;
        var cheep = new Cheep("This", "Should not be here", 1690981487);

        // Act
        await csv.Store(cheep);
        await csv.ArrangeTestDatabase();
        var data = await csv.Read(10);

        // Assert
        Assert.StartsWith("ropf", data[0].Author);
        Assert.EndsWith("Cheeping cheeps on Chirp :)", data[data.Count - 1].Message);
    }
}