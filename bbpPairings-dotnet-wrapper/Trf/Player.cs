using System.Text;
using bbpPairings_dotnet_wrapper.Utils;

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
        // var resultsFormatted = Results.Select(r =>
        // {
        //     var opponent = r.OpponentNumber == 0 ? "    " : r.OpponentNumber.Pad(4, true);
        //     var color = r.IsWhite is null ? ' ' : (r.IsWhite.Value ? 'w' : 'b');
        //     var res = r.Result;
        //     return $"{opponent} {color} {res}";
        // });
        //
        // var parts = new List<string>
        // {
        //     "001",
        //     Number.Pad(4, true),
        //     string.Empty.Pad(1, true),          // Sex
        //     Title.Pad(3, true),
        //     Name.Pad(29, false),
        //     Rating.Pad(4, true),
        //     Federation.Pad(4, true),
        //     string.Empty.Pad(4, true),          // FIDE Number
        //     string.Empty.Pad(19, true),         // Birthdate
        //     Points.ToString("F1").Pad(4, true),
        //     Rank.Pad(4, false)
        // };
        //
        // // Add results, joined with two spaces
        // var resultsStr = string.Join("  ", resultsFormatted);
        // parts.Add(resultsStr);
        //
        // return string.Join(" ", parts);
        var results = Results.Select(r =>
        {
            var opponent = r.OpponentNumber == 0 ? "    " : r.OpponentNumber.Pad( 4, true);
            var res = r.Result;
            var color = r.IsWhite is null ? ' ' : r.IsWhite.Value ? 'w' : 'b';
            return $"{opponent} {color} {res}";
        });
        var number = Number.Pad(4, true);
        var sex = string.Empty.Pad(1, true);
        var title = Title.Pad(3, true);
        var name = Name.Pad( 29, false);
        var fideRating = Rating.Pad( 4, true);
        var federation = Federation.Pad( 4, true);
        var fideNumber = string.Empty.Pad( 4, true);
        var birthdate = string.Empty.Pad( 19, true);
        var points = Points.ToString("F1").Pad( 4, true);
        var rank = Rank.Pad( 4, false);
        var resultsStr = string.Join("  ", results);
        var s = $"001 {number} {sex} {title} {name} {fideRating} {federation} {fideNumber} {birthdate} {points} {rank}  {resultsStr}";
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