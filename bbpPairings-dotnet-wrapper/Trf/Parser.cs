namespace bbpPairings_dotnet_wrapper.Trf;

public class Parser
{
    public Tournament ParseTrfFile(string filePath)
    {
        Tournament tournament = new Tournament();
        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            if (line.Length < 3) continue;

            string code = line.Substring(0, 3);
            string content = line.Length > 4 ? line.Substring(4).Trim() : "";

            switch (code)
            {
                case "012":
                    tournament.Name = content;
                    break;
                case "022":
                    tournament.Location = content;
                    break;
                case "032":
                    tournament.StartDate = ParseDate(content);
                    break;
                case "042":
                    tournament.EndDate = ParseDate(content);
                    break;
                case "052":
                    tournament.PlayerCount = int.Parse(content);
                    break;
                case "062":
                    tournament.TotalRounds = int.Parse(content);
                    break;
                case "072":
                    tournament.PlayedRounds = int.Parse(content);
                    break;
                case "092":
                    tournament.InitialColor = content.ToLower()[0];
                    break;
                case "122":
                    tournament.Arbiter = content;
                    break;
                case "132":
                    tournament.ContactInfo = content;
                    break;
                case "001":
                    tournament.Players.Add(ParsePlayer(line, tournament.PlayedRounds));
                    break;
            }
        }

        return tournament;
    }

    private DateTime ParseDate(string dateString)
    {
        // Định dạng TRF: YYYY.MM.DD
        string[] parts = dateString.Split('.');
        if (parts.Length == 3)
        {
            return new DateTime(
                int.Parse(parts[0]),
                int.Parse(parts[1]),
                int.Parse(parts[2])
            );
        }
        return DateTime.MinValue;
    }

    private Player ParsePlayer(string line, int playedRounds)
    {
        string? TryGet(int start, int? end = null)
        {
            var charsCount = line.Length;
            if (start > charsCount || end > charsCount)
            {
                return null;
            }

            start -= 1;
            end -= 1;
            string c;
            if (end is null)
            {
                c = line.Substring(start, 1);
            }
            else
            {
                c = line.Substring(start, end.Value - start + 1);
            }

            return c.Trim();
        }
        
        // must be 001
        var code = TryGet(1, 3);
        
        var number = TryGet(5, 8);
        var sex = TryGet(10);
        var title = TryGet(11, 13);
        var name = TryGet(15, 47);
        var fideRating = TryGet(49, 52);
        var fideFederation = TryGet(54, 56);
        var fideNumber = TryGet(58, 68);
        var birthdate = TryGet(70, 79);
        var points = TryGet(81, 84);
        var rank = TryGet(86, 89);
        if (string.IsNullOrEmpty(number) ||
            string.IsNullOrEmpty(name) ||
            string.IsNullOrEmpty(points) ||
            string.IsNullOrEmpty(rank))
        {
            throw new InvalidDataException();
        }
        
        Player player = new ()
        {
            Number = int.Parse(number),
            Points = float.Parse(points),
            Rank = int.Parse(rank),
            Results = [],
        };


        int startLine = 92;
        while (line.Length >= startLine + 8)
        {
            var playerId = TryGet(startLine, startLine + 3);
            var color = TryGet(startLine + 5);
            var result = TryGet(startLine + 7);

            if (string.IsNullOrEmpty(playerId) || string.IsNullOrEmpty(result))
            {
                continue;
            }

            GameResult gameResult = new()
            {
                OpponentNumber = int.Parse(playerId),
                Result = result[0],
            };
            
            player.Results.Add(gameResult);

            startLine += 10;
        }

        // // Bỏ qua phần "001" nếu có
        // int startIndex = parts[0] == "001" ? 1 : 0;
        //
        // try
        // {
        //     // Số thứ tự người chơi
        //     player.Number = int.Parse(parts[startIndex++]);
        //
        //     // Tên người chơi (có thể bao gồm nhiều phần)
        //     StringBuilder nameBuilder = new StringBuilder();
        //     while (startIndex < parts.Length &&
        //            !Regex.IsMatch(parts[startIndex], @"^[mfMF]$") && // Không phải giới tính
        //            !Regex.IsMatch(parts[startIndex], @"^[GIMWCF][MGWCF]?$") && // Không phải title
        //            !Regex.IsMatch(parts[startIndex], @"^\d{4}$")) // Không phải rating
        //     {
        //         nameBuilder.Append(parts[startIndex++]).Append(" ");
        //     }
        //
        //     player.Name = nameBuilder.ToString().Trim();
        //
        //     // Giới tính (không bắt buộc)
        //     if (startIndex < parts.Length && Regex.IsMatch(parts[startIndex], @"^[mfMF]$"))
        //     {
        //         player.Sex = parts[startIndex++][0];
        //     }
        //
        //     // Danh hiệu (không bắt buộc)
        //     if (startIndex < parts.Length && Regex.IsMatch(parts[startIndex], @"^[GIMWCF][MGWCF]?$"))
        //     {
        //         player.Title = parts[startIndex++];
        //     }
        //
        //     // Rating (có thể không có)
        //     if (startIndex < parts.Length && Regex.IsMatch(parts[startIndex], @"^\d{4}$"))
        //     {
        //         player.Rating = int.Parse(parts[startIndex++]);
        //     }
        //
        //     // Liên đoàn (có thể không có)
        //     if (startIndex < parts.Length && Regex.IsMatch(parts[startIndex], @"^[A-Z]{2,3}$"))
        //     {
        //         player.Federation = parts[startIndex++];
        //     }
        //
        //     // ID (có thể không có)
        //     if (startIndex < parts.Length && Regex.IsMatch(parts[startIndex], @"^\d{4,}$"))
        //     {
        //         player.Id = parts[startIndex++];
        //     }
        //
        //     // Điểm
        //     if (startIndex < parts.Length && float.TryParse(parts[startIndex], out float score))
        //     {
        //         player.Score = score;
        //         startIndex++;
        //     }
        //
        //     // Điểm phụ (2-3 giá trị)
        //     int tiebreakCount = 0;
        //     while (startIndex < parts.Length &&
        //            float.TryParse(parts[startIndex], out float tieBreak) &&
        //            tiebreakCount < 3)
        //     {
        //         player.TieBreaks.Add(tieBreak);
        //         startIndex++;
        //         tiebreakCount++;
        //     }
        //
        //     // Kết quả trận đấu - ĐÂY LÀ PHẦN QUAN TRỌNG
        //     player.Results.Clear(); // Đảm bảo danh sách rỗng trước khi thêm
        //
        //     // Tính toán lại số vòng dựa trên số phần còn lại trong mảng
        //     int remainingFields = parts.Length - startIndex;
        //     int roundsToProcess = Math.Min(remainingFields, playedRounds);
        //
        //     Console.WriteLine($"Xử lý {roundsToProcess} vòng cho người chơi {player.Number}");
        //
        //     for (int i = 0; i < roundsToProcess; i++)
        //     {
        //         string resultStr = parts[startIndex + i];
        //         Console.WriteLine($"Kết quả vòng {i + 1}: '{resultStr}'");
        //
        //         GameResult gameResult = ParseGameResult(resultStr);
        //         player.Results.Add(gameResult);
        //     }
        //
        //     if (player.Results.Count == 0 && playedRounds > 0)
        //     {
        //         Console.WriteLine($"Cảnh báo: Không tìm thấy kết quả trận đấu cho người chơi {player.Number}");
        //     }
        // }
        // catch (Exception ex)
        // {
        //     Console.WriteLine($"Lỗi khi phân tích dữ liệu người chơi: {ex.Message}");
        //     Console.WriteLine($"Dữ liệu: {line}");
        // }

        return player;
    }

    private GameResult ParseGameResult(string resultStr)
    {
        GameResult result = new GameResult();
        
        if (string.IsNullOrEmpty(resultStr) || resultStr == "-")
        {
            result.Result = '-';
            return result;
        }
        
        // Xử lý các bye đặc biệt
        if (resultStr == "+" || resultStr == "U")
        {
            result.Result = '+';
            return result;
        }
        
        if (resultStr == "+=" || resultStr == "H")
        {
            result.Result = 'h';
            return result;
        }
        
        if (resultStr == "z" || resultStr == "Z")
        {
            result.Result = 'z';
            return result;
        }
        
        if (resultStr == "f" || resultStr == "F")
        {
            result.Result = 'f';
            return result;
        }
        
        // Xử lý kết quả thông thường: w0001, b0002, d0003, ...
        if (resultStr.Length >= 2)
        {
            result.Result = resultStr[0];
            string opponentStr = resultStr.Substring(1);
            
            if (int.TryParse(opponentStr, out int opponent))
            {
                result.OpponentNumber = opponent;
            }
        }
        
        return result;
    }

    // Phương thức để viết ra tệp TRF
    public void WriteTrfFile(Tournament tournament, string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Viết các thông tin tiêu đề
            writer.WriteLine($"012 {tournament.Name}");
            writer.WriteLine($"022 {tournament.Location}");
            writer.WriteLine($"032 {tournament.StartDate:yyyy.MM.dd}");
            writer.WriteLine($"042 {tournament.EndDate:yyyy.MM.dd}");
            writer.WriteLine($"052 {tournament.PlayerCount}");
            writer.WriteLine($"062 {tournament.TotalRounds}");
            writer.WriteLine($"072 {tournament.PlayedRounds}");
            writer.WriteLine($"092 {tournament.InitialColor}");
            writer.WriteLine($"122 {tournament.Arbiter}");
            writer.WriteLine($"132 {tournament.ContactInfo}");
            
            // Viết thông tin người chơi
            foreach (var player in tournament.Players)
            {
                writer.Write($"001 {player.Number,4} {player.Name,-25} {player.Sex} ");
                
                if (!string.IsNullOrEmpty(player.Title))
                    writer.Write($"{player.Title,4} ");
                else
                    writer.Write("     ");
                    
                writer.Write($"{player.Rating,4} {player.Federation,-3} {player.Id,-10} {player.Points,4:F1} ");
                
                // Viết hệ số phụ
                foreach (var tieBreak in player.TieBreaks)
                {
                    writer.Write($"{tieBreak,6:F1} ");
                }
                
                // Viết kết quả các vòng
                foreach (var result in player.Results)
                {
                    writer.Write($"{result} ");
                }
                
                writer.WriteLine();
            }
        }
    }
}