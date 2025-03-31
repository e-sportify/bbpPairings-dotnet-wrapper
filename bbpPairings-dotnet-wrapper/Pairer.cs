using System.Diagnostics;
using bbpPairings_dotnet_wrapper.Files;
using bbpPairings_dotnet_wrapper.Trf;
using Microsoft.Extensions.Options;

namespace bbpPairings_dotnet_wrapper;

internal sealed class Pairer(IOptionsSnapshot<ExecutableOptions> optionsSnapshot)
{
    private string GetPairingSystemFlag(PairingSystem pairingSystem)
    {
        return pairingSystem switch
        {
            PairingSystem.Dutch => "--dutch",
            PairingSystem.Burstein => "--burstein",
            PairingSystem.Fast => "--fast",
            _ => "--dutch",
        };
    }
    public async Task<string> GenerateRandomTournament(
        PairingSystem pairingSystem,
        int playersNumber,
        int roundsNumber)
    {
        using ConfigurationFile inputFile = ConfigurationFile.Create(playersNumber, roundsNumber);
        using TempFile outputFile = TempFile.Create();
        
        await inputFile.Writer.WriteAsync(inputFile.ToString());
        await inputFile.Writer.FlushAsync();

        string pairingSystemFlag = GetPairingSystemFlag(pairingSystem);
        ProcessStartInfo psi = new()
        {
            FileName = optionsSnapshot.Value.FileName,
            Arguments = $"{pairingSystemFlag} -g {inputFile.FilePath} -o {outputFile.FilePath}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false, // Required for redirection
            CreateNoWindow = true, // Prevents opening a new window
        };

        using Process process = new();

        process.StartInfo = psi;
        process.Start();

        await process.WaitForExitAsync();

        string output = await outputFile.Reader.ReadToEndAsync();
        var parser = new Parser();
        var res = parser.ParseTrfFile(outputFile.FilePath);
        
        return output;
    }
}