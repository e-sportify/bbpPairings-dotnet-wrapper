using bbpPairings_dotnet_wrapper.Trf;
using FastEndpoints;

namespace bbpPairings_dotnet_wrapper.Endpoints;

public sealed class Pair(Pairer pairer) : Endpoint<Pair.Request, Pair.Response>
{
    public override void Configure()
    {
        Post("/pair");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        Tournament tournament = new()
        {
            Name = "abc",
            TotalRounds = req.NumberOfRounds,
        };
        List<Player> players = [];
        Dictionary<Guid, int> playersMap = [];
        int playerNumber = 1;
        foreach (var player in req.Players)
        {
            playersMap.Add(player.Id, playerNumber++);
        }

        foreach (var p in req.Players)
        {
            Player player = new()
            {
                Number = playersMap[p.Id],
                Results = [],
                Points = p.Points,
                Name = playersMap[p.Id].ToString(),
            };
            bool hasPlayed = false;
            for (int i = 1; i <= req.NumberOfRoundsPlayed; i++)
            {
                var m = p.Matches.FirstOrDefault(m => m.RoundIndex == i);
                var gameResult = new GameResult
                {
                    OpponentNumber = 0,
                    Result = hasPlayed ? '-' : 'H',
                };
                if (m is not null)
                {
                    gameResult.OpponentNumber = playersMap[m.OpponentId];
                    gameResult.Result = m.Won is null ? '=' : m.Won.Value ? '1' : '0';
                    gameResult.IsWhite = m.IsWhite;
                }

                player.Results.Add(gameResult);
            }

            players.Add(player);
        }

        players = players
            .OrderByDescending(p => p.Points)
            .ThenBy(p => p.Number).ToList();

        var rankCounter = 1;
        foreach (var player in players)
        {
            player.Rank = rankCounter++;
        }

        tournament.Players = players;

        var output = await pairer.Pair(tournament, PairingSystem.Dutch);
        var idMaps = playersMap.ToDictionary(kv => kv.Value, kv => kv.Key);
        var pairs = output
            .Select(l => l.Split(" "))
            .Where(ps => ps.Length == 2)
            .Select(ps =>
            {
                var p1 = int.Parse(ps[0]);
                var p2 = int.Parse(ps[1]);
                return new Response.Pair
                {
                    Player1 = idMaps[p1],
                    Player2 = idMaps[p2],
                };
            })
            .ToList();

        await SendAsync(new Response
        {
            Pairs = pairs,
        }, cancellation: ct);
    }

    public class Request
    {
        public int NumberOfRounds { get; set; }
        public int NumberOfRoundsPlayed { get; set; }
        public ICollection<Player> Players { get; set; } = [];

        public class Player
        {
            public Guid Id { get; set; }
            public float Points { get; set; }
            public ICollection<Match> Matches { get; set; } = [];

            public sealed class Match
            {
                public int RoundIndex { get; set; }

                public Guid OpponentId { get; set; }

                // null means draw
                public bool? Won { get; set; }
                public bool IsWhite { get; set; }
            }
        }
    }

    public class Response
    {
        public ICollection<Pair> Pairs { get; set; } = [];

        public class Pair
        {
            public Guid Player1 { get; set; }
            public Guid Player2 { get; set; }
        }
    }
}