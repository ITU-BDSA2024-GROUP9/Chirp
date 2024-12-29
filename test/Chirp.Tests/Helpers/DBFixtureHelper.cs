namespace Chirp.Tests.Helpers;

public class DBFixtureHelper : IDisposable
{
    public DatabaseHelper DatabaseHelper { get; }

    public DBFixtureHelper()
    {
        // Initialize DatabaseHelper with the required connection string or other inputs
        string connectionString = "Filename=:memory:"; // or any other value
        DatabaseHelper = new DatabaseHelper(connectionString);
    }

    public void Dispose()
    {
        // Clean up any resources if needed
        DatabaseHelper.Dispose();
    }
}
