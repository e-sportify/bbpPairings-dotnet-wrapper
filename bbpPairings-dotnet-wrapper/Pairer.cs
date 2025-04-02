using System.Diagnostics;
using bbpPairings_dotnet_wrapper.Files;
using bbpPairings_dotnet_wrapper.Trf;
using Microsoft.Extensions.Options;

namespace bbpPairings_dotnet_wrapper;

public sealed class Pairer(IOptionsSnapshot<ExecutableOptions> optionsSnapshot)
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
        using ConfigurationFile inputFile = ConfigurationFile.Create(
            playersNumber,
            roundsNumber,
            40,
            5,
            null,
            null,
            1,
            0.5f,
            0,
            0.7f,
            0.3f,
            0.4f);
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

        return output;
    }

    public async Task<List<string>> Pair(Tournament tournament, PairingSystem pairingSystem)
    {
        using TempFile inputFile = TempFile.Create();
        using TempFile outputFile = TempFile.Create();

        var s = tournament.ToString();
        await inputFile.Writer.WriteAsync(s);
        await inputFile.Writer.FlushAsync();

        string pairingSystemFlag = GetPairingSystemFlag(pairingSystem);
        ProcessStartInfo psi = new()
        {
            FileName = optionsSnapshot.Value.FileName,
            Arguments = $"{pairingSystemFlag} {inputFile.FilePath} -p {outputFile.FilePath}",
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

        return output.Split("\n").ToList();
    }
}