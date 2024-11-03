namespace Chirp.Core.Helpers;

using System.Diagnostics;
using System.Threading.Tasks;

public static class MyEndToEndUtil
{
    private static Process _serverProcess;

    public static async Task StartServer()
    {
        _serverProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "run --project D:/Coding Projects/Chirp/src/Chirp.Razor/Program.cs --urls http://localhost:5273/",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };
        
        _serverProcess.Start();
        bool isServerReady = false;
        int maxAttempts = 10;
        int attempt = 0;

        while (!isServerReady && attempt < maxAttempts)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync("http://localhost:5273/");
                    if (response.IsSuccessStatusCode)
                    {
                        isServerReady = true;
                    }
                }
            }
            catch
            {
                // Server not ready yet
                await Task.Delay(1000); // Wait for 1 second before trying again
            }
            attempt++;
        }

        if (!isServerReady)
        {
            throw new Exception("Server did not start within the expected time.");
        }    }

    public static void StopServer()
    {
        if (_serverProcess != null && !_serverProcess.HasExited)
        {
            _serverProcess.Kill();
            _serverProcess.WaitForExit();
            _serverProcess.Dispose();
        }
    }
}
