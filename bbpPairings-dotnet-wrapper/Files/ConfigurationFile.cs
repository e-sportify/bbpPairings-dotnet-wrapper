namespace bbpPairings_dotnet_wrapper.Files;

public sealed class ConfigurationFile : TempFile
{
    public int PlayersNumber { get; set; } = 5;
    public int RoundsNumber { get; set; } = 3;

    public int? DrawPercentage { get; set; } = null;
    public int? ForfeitRate { get; set; } = null;
    public int? RetiredRate { get; set; } = null;
    public int? HalfPointByeRate { get; set; } = null;

    public float? PointsForWin { get; set; } = 1;
    public float? PointsForDraw { get; set; } = 0.5f;
    public float? PointsForLoss { get; set; } = 0;
    // zero point bye
    public float? PointsForZPB { get; set; } = 0;
    public float? PointsForForfeitLoss { get; set; } = 0;
    // pairing allocated bye
    public float? PointsForPAB { get; set; } = 1;

    public override string ToString()
    {
        string? TryGet(string name, object? value) => value is not null ? $"{name}={value}" : null;

        List<string?> fields =
        [
            TryGet(nameof(PlayersNumber), PlayersNumber),
            TryGet(nameof(RoundsNumber), RoundsNumber),
            TryGet(nameof(DrawPercentage), DrawPercentage),
            TryGet(nameof(ForfeitRate), ForfeitRate),
            TryGet(nameof(RetiredRate), RetiredRate),
            TryGet(nameof(HalfPointByeRate), HalfPointByeRate),
            TryGet(nameof(PointsForWin), PointsForWin),
            TryGet(nameof(PointsForDraw), PointsForDraw),
            TryGet(nameof(PointsForLoss), PointsForLoss),
            TryGet(nameof(PointsForZPB), PointsForZPB),
            TryGet(nameof(PointsForForfeitLoss), PointsForForfeitLoss),
            TryGet(nameof(PointsForPAB), PointsForPAB),
        ];

        return string.Join("\n", fields.Where(f => f is not null));
    }

    public static ConfigurationFile Create(
        int playersNumber,
        int roundsNumber,
        int? drawPercentage = null,
        int? forfeitRate = null,
        int? retiredRate = null,
        int? halfPointByeRate = null,
        float? pointsForWin = null,
        float? pointsForDraw = null,
        float? pointsForLoss = null,
        float? pointsForZPB = null,
        float? pointsForForfeitLoss = null,
        float? pointsForPAB = null)
    {
        return new ConfigurationFile
        {
            PlayersNumber = playersNumber, 
            RoundsNumber = roundsNumber,
            DrawPercentage = drawPercentage,
            ForfeitRate = forfeitRate,
            RetiredRate = retiredRate,
            HalfPointByeRate = halfPointByeRate,
            PointsForWin = pointsForWin,
            PointsForDraw = pointsForDraw,
            PointsForLoss = pointsForLoss,
            PointsForZPB = pointsForZPB,
            PointsForForfeitLoss = pointsForForfeitLoss,
            PointsForPAB = pointsForPAB,
        };
    }
}