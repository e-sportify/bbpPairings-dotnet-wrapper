using FastEndpoints;

namespace bbpPairings_dotnet_wrapper.Endpoints;

public sealed class GenerateDummyTournament(
    Pairer pairer) : Endpoint<
    GenerateDummyTournament.Request,
    IEnumerable<string>>
{
    public override void Configure()
    {
        Post("/generate-dummy-tournament");
        AllowAnonymous();
        Description(b => b
            .Produces(200));
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        string output = await pairer.GenerateRandomTournament(
            PairingSystem.Dutch,
            req.PlayersNumber,
            req.RoundsNumber);

        await SendAsync(output.Split("\r"), 200, ct);
    }

    public sealed class Request
    {
        public int PlayersNumber { get; set; }
        public int RoundsNumber { get; set; }
    }
}