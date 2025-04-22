namespace bbpPairings_dotnet_wrapper.Utils;

public static class StringExtensions
{
    public static string Pad(this object? value, int padding, bool isLeftPad)
    {
        if (value is null)
        {
            return string.Empty.PadLeft(padding);
        }

        return isLeftPad ? value.ToString()!.PadLeft(padding) : value.ToString()!.PadRight(padding);
    }
}