using System.Text;

namespace bbpPairings_dotnet_wrapper.Trf;

public class Player
{
    public int Number { get; set; }
    public string Name { get; set; } = string.Empty;
    public char Sex { get; set; } // 'm' hoặc 'f'
    public string Title { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Federation { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public float Points { get; set; }
    public int Rank { get; set; }
    public List<float> TieBreaks { get; set; } = [];
    public List<GameResult> Results { get; set; } = [];
    public bool IsAdvanced { get; set; }
    public bool IsEliminated { get; set; }

    public override string ToString()
    {
        var results = Results.Select(r =>
        {
            var opponent = r.OpponentNumber == 0 ? "    " : TryGet(r.OpponentNumber, 4, true);
            var res = r.Result;
            var color = r.IsWhite is null ? ' ' : r.IsWhite.Value ? 'w' : 'b';
            return $"{opponent} {color} {res}";
        });
        var number = TryGet(Number, 4, true);
        var sex = TryGet(string.Empty, 1, true);
        var title = TryGet(Title, 3, true);
        var name = TryGet(Name, 29, false);
        var fideRating = TryGet(Rating, 4, true);
        var federation = TryGet(Federation, 4, true);
        var fideNumber = TryGet(string.Empty, 4, true);
        var birthdate = TryGet(string.Empty, 19, true);
        var points = TryGet(Points.ToString("F1"), 4, true);
        var rank = TryGet(Rank, 4, false);
        var resultsStr = string.Join("  ", results);
        var s =
            $"001 {number} {sex} {title} {name} {fideRating} {federation} {fideNumber} {birthdate} {points} {rank}  {resultsStr}";
        return s;
    }

    public string ToAnotherString()
    {
        if (!IsAdvanced) return string.Empty;

        string number = TryGet(Number, 4, true);
        string sex = TryGet(string.Empty, 1, true);
        string title = TryGet(string.Empty, 3, true);
        string advancedString = IsAdvanced ? "1" : "2";

        return $"XXS {number} {sex} {title} {advancedString}";
    }

    private string TryGet(object? value, int padding, bool isLeftPad)
    {
        if (value is null)
        {
            return string.Empty.PadLeft(padding);
        }

        return isLeftPad ? value.ToString().PadLeft(padding) : value.ToString().PadRight(padding);
    }
}