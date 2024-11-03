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
                Arguments = "run --project path/to/your/project",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };
        
        _serverProcess.Start();
        await Task.Delay(5000); // Wait for server to be fully operational, adjust time as necessary
    }

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
