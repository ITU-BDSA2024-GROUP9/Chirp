using Chirp.Core.Classes;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Chirp.CSVDBService;

public class UnitTest1
{
    [Fact]
    async public void TestRead()
    {
        var db = DatabaseClientService<Cheep>.Instance;

        //Act
        var it = await csv.Read(1);
        Cheep cheep = new Cheep("ropf", "Hello, BDSA students!", 1690891760);

        //Assert
        Assert.Equals(cheep, it.First());
    }


    [Fact]
    async public void TestDelete()
    {
        
    }

    [Fact]
    async public void TestWrite()
    {
        var db = DatabaseClientService<Cheep>.Instance;

        Cheep cheep = new Cheep("ropf", "Hello, BDSA students!", 1690891760);
        db.store(cheep);
        
        
    }

}