using System.ComponentModel;
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
        Dictionary<Guid, int> playerNumbersMap = req.Players
            .Select((player, index) => new { player.Id, Number = index + 1 })
            .ToDictionary(p => p.Id, p => p.Number);

        foreach (Request.Player reqPlayer in req.Players)
        {
            Player player = new()
            {
                Number = playerNumbersMap[reqPlayer.Id],
                Results = [],
                Points = reqPlayer.Points,
                Name = playerNumbersMap[reqPlayer.Id].ToString(),
            };
            for (int i = 1; i <= req.NumberOfRoundsPlayed; i++)
            {
                Request.Player.Match? m = reqPlayer.Matches.FirstOrDefault(m => m.RoundIndex == i);
                GameResult gameResult = new()
                {
                    OpponentNumber = 0,
                    Result = '-',
                };
                if (m is not null)
                {
                    gameResult.OpponentNumber = playerNumbersMap[m.OpponentId];
                    // gameResult.Result = m.Won is null ? '=' : m.Won.Value ? '1' : '0';
                    gameResult.Result = m.Result switch
                    {
                        Request.Player.MatchResult.Won => '1',
                        Request.Player.MatchResult.Lost => '0',
                        Request.Player.MatchResult.Draw => '=',
                        Request.Player.MatchResult.WonForfeited => '+',
                        Request.Player.MatchResult.LostForfeited => '-',
                        Request.Player.MatchResult.FullPointBye => 'F',
                        Request.Player.MatchResult.HalfPointBye => 'H',
                        Request.Player.MatchResult.PairingAllocatedBye => 'U',
                        Request.Player.MatchResult.ZeroPointBye => 'Z',
                        _ => throw new InvalidEnumArgumentException(),
                    };
                    
                    gameResult.IsWhite = m.IsWhite;
                }

                player.Results.Add(gameResult);
            }

            players.Add(player);
        }

        players = players
            .OrderByDescending(p => p.Points)
            .ThenBy(p => p.Number)
            .ToList();

        int rankCounter = 1;
        players.ForEach(player => player.Rank = rankCounter++);

        tournament.Players = players;

        List<string> output = await pairer.Pair(tournament, req.PairingSystem);
        Dictionary<int, Guid> playerIdsMap = playerNumbersMap.ToDictionary(kv => kv.Value, kv => kv.Key);
        List<Response.Pair> pairs = output
            .Select(l => l.Split(" "))
            .Where(ps => ps.Length == 2)
            .Select(ps =>
            {
                int p1 = int.Parse(ps[0]);
                int p2 = int.Parse(ps[1]);
                return new Response.Pair
                {
                    Player1 = playerIdsMap.TryGetValue(p1, out var p1Id) ? p1Id : Guid.Empty,
                    Player2 = playerIdsMap.TryGetValue(p2, out var p2Id) ? p2Id : Guid.Empty,
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
        public PairingSystem PairingSystem { get; set; }
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
                public MatchResult Result { get; set; }
                public bool IsWhite { get; set; }
            }
            public enum MatchResult
            {
                Won,
                Lost,
                Draw,
                WonForfeited,
                LostForfeited,
                HalfPointBye,
                FullPointBye,
                PairingAllocatedBye,
                ZeroPointBye,
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