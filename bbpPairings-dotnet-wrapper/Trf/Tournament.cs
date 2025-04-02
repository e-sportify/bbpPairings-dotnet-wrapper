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
    public List<Player> Players { get; set; } = [];
    public override string ToString()
    {
        string name = $"012 {Name}";
        string totalRounds = $"XXR {TotalRounds}";
        string initColor = $"XXC white1";
        var players = Players.Select(p => p.ToString());
        var s = $"{name}\n{totalRounds}\n{initColor}\n{string.Join("\n", players)}";
        return s;
    }
}