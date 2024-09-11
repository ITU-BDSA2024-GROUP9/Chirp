using System.Diagnostics;
using System.IO;
using Chirp.Core.Classes;
using SimpleDB.Services;
using Xunit;

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
        Assert.EndsWith("Hello, BDSA students!\r", fstCheep);
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
		Assert.Equal("cheeping\r\n", output);
    }
}