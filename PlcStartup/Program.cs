// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
// Cmd: dotnet run -c release --no-build --project ~/Hydroponic/Hydroponic.Plc/Hydroponic.Plc.csproj
string dotnetCommand = "dotnet run -c release --no-build --project ~/Hydroponic/Hydroponic.Plc/Hydroponic.Plc.csproj";
string pythonCommand = "python3 ~/Hydroponic.Py/main.py";

bool connected;
do
{
    connected = await HasInternetConnection();
    if (connected == false)
    {
        Console.WriteLine("No internet connection. Retrying in 5 seconds...");
        await Task.Delay(5000);
    }
} while (connected == false);

Console.WriteLine("Internet connection established.");
await RunCommand(dotnetCommand);
await RunCommand(pythonCommand);


static async Task RunCommand(string command)
{
    ProcessStartInfo psi = new ProcessStartInfo
    {
        FileName = "/bin/bash",
        Arguments = $"-c \"{command}\"",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    using (Process process = new())
    {
        process.StartInfo = psi;
        process.OutputDataReceived += (sender, args) => { };
        process.ErrorDataReceived += (sender, args) => { };
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync();
    }

}


static async Task<bool> HasInternetConnection()
{
    try
    {
        using (var client = new HttpClient())
        {
            var response = await client.GetAsync("https://www.google.com");
            return response.IsSuccessStatusCode;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception: {ex.Message}");
        return false;
    }
}
