using Chirp.Core.Classes;

namespace Chirp.CLI.Tests;

public class UnitTests  
{
    [Fact]
    public void TestCheep()
    {
        // Arrange
        var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        // Act
        Cheep _cheep = new("a", "b", time);
        // Assert
        Assert.Equal("a", _cheep.Author);
        Assert.Equal("b", _cheep.Message);
        Assert.Equal(time, _cheep.Timestamp);
    }


}