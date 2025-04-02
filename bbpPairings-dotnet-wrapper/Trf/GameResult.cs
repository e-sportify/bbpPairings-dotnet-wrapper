namespace bbpPairings_dotnet_wrapper.Trf;

public class GameResult
{
    public char Result { get; set; }

    public ResultEnum GetResult()
    {
        return Result switch
        {
            '-' or '+' => ResultEnum.Forfeit,
            'W' or 'w' or '1' => ResultEnum.Win,
            'D' or 'd' or '=' => ResultEnum.Draw,
            'L' or 'l' or '0' => ResultEnum.Loss,
            'H' or 'h' or 'F' or 'f' or 'U' or 'u' => ResultEnum.Bye,
            'Z' or 'z' => ResultEnum.Blank,
        };
    }

    public int OpponentNumber { get; set; } // 0 nếu là bye
    public bool? IsWhite { get; set; } // if null then = '-'
        
    public override string ToString()
    {
        if (OpponentNumber == 0)
        {
            if (Result is '+' or 'U')
                return "+";
            if (Result is 'h' or 'H')
                return "+=";
            if (Result is 'z' or 'Z')
                return "z";
            if (Result is 'f' or 'F')
                return "f";
            return Result.ToString();
        }
            
        return Result + OpponentNumber.ToString("D4");
    }
}

public enum ResultEnum
{
    Forfeit,
    Win,
    Draw,
    Loss,
    Bye,
    Blank,
}