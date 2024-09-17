using System.Diagnostics;
using Chirp.Core.Classes;
using SimpleDB.Services;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

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
public class End2End
{
    [Fact]
    public void TestReadCheep()
    {
        // Arrange
        var projectDir = Directory.GetCurrentDirectory();
        var chirpCliBinPath = Path.GetFullPath(Path.Combine(projectDir, "..", "..", "..", "..", "..", "src", "Chirp.CLI", "bin", "Debug", "net8.0", "Chirp.CLI.dll"));

        // Act
        string output = "";
        using (var process = new Process())
        {
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = $"exec \"{chirpCliBinPath}\" read 10";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            // Synchronously read the standard output of the spawned process.
            StreamReader reader = process.StandardOutput;
            output = reader.ReadToEnd();
            process.WaitForExit();
        }

        string[] cheeps = output.Split('\n');

        // Assert
        Assert.NotEmpty(cheeps);
        string fstCheep = cheeps[0];
        Assert.StartsWith("ropf", fstCheep);
        Assert.EndsWith("Hello, BDSA students!", fstCheep.TrimEnd('\r', '\n'));
    }

    [Fact]
    async public void TestStoreCheep()
    {
        // Arrange
        var csv = CSVDatabaseService<Cheep>.Instance;
        await csv.ArrangeTestDatabase();
        var projectDir = Directory.GetCurrentDirectory();
        var chirpCliBinPath = Path.GetFullPath(Path.Combine(projectDir, "..", "..", "..", "..", "..", "src", "Chirp.CLI", "bin", "Debug", "net8.0", "Chirp.CLI.dll"));

        // Act
        string output = "";
        using (var process = new Process())
        {
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = $"exec \"{chirpCliBinPath}\" cheep \"e2eTest\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            // Synchronously read the standard output of the spawned process.
            StreamReader reader = process.StandardOutput;
            output = reader.ReadToEnd();
            process.WaitForExit();
        }

        // Assert
        Assert.Contains("cheeping", output);
        await csv.ArrangeTestDatabase();
    }
}