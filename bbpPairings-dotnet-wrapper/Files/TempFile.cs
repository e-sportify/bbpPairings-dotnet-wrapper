namespace bbpPairings_dotnet_wrapper.Files;

public class TempFile : IDisposable
{
    public string FilePath { get; set; }
    public StreamReader Reader { get; set; }
    public StreamWriter Writer { get; set; }

    protected TempFile()
    {
        FilePath = Path.GetTempFileName();
        Reader = new StreamReader(FilePath);
        Writer = new StreamWriter(FilePath);
    }

    public static TempFile Create()
    {
        return new TempFile();
    }

    public void Dispose()
    {
        Reader.Dispose();
        Writer.Dispose();
        File.Delete(FilePath);
    }
}