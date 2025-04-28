using bbpPairings_dotnet_wrapper.Utils;

namespace bbpPairings_dotnet_wrapper.Trf;

public class Tournament
{
    public string Name { get; set; }
    public string Location { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int PlayerCount { get; set; }
    public int TotalRounds { get; set; }
    public int PlayedRounds { get; set; }
    public char InitialColor { get; set; } // 'w' hoặc 'b'
    public string Arbiter { get; set; }
    public string ContactInfo { get; set; }
    public float? PointsForWin { get; set; }
    public float? PointsForDraw { get; set; }

    public float? PointsForLoss { get; set; }

    // zero point bye
    public float? PointsForZPB { get; set; }

    public float? PointsForForfeitLoss { get; set; }

    // pairing allocated bye
    public float? PointsForPAB { get; set; }
    public List<Player> Players { get; set; } = [];

    public override string ToString()
    {
        var lines = new List<string>
        {
            $"012 {Name}",
            $"XXR {TotalRounds}",
            $"XXC white1"
        };

        if (PointsForWin is not null) lines.Add($"BBW {PointsForWin.Pad(4, isLeftPad: true)}");
        if (PointsForDraw is not null) lines.Add($"BBD {PointsForDraw.Pad(4, isLeftPad: true)}");
        if (PointsForLoss is not null) lines.Add($"BBL {PointsForLoss.Pad(4, isLeftPad: true)}");
        if (PointsForZPB is not null) lines.Add($"BBZ {PointsForZPB.Pad(4, isLeftPad: true)}");
        if (PointsForForfeitLoss is not null) lines.Add($"BBF {PointsForForfeitLoss.Pad(4, isLeftPad: true)}");
        if (PointsForPAB is not null) lines.Add($"BBU {PointsForPAB.Pad(4, isLeftPad: true)}");

        // Add players
        lines.AddRange(Players.Select(p => p.ToString()));

        // Add advanced/eliminated players if any
        var advanced = Players
            .Where(pl => pl.IsAdvanced || pl.IsEliminated)
            .Select(p => p.ToAnotherString())
            .ToList();

        if (advanced.Count > 0)
        {
            lines.Add(""); // Blank line before advanced players
            lines.AddRange(advanced);
        }

        return string.Join("\n", lines);
    }
}