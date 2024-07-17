namespace FSCore.Matches;

/// <summary>
/// Basic ID generator, generates incementing numerical values as strings
/// </summary>
public class BasicIDGenerator : IIDGenerator
{
    /// <summary>
    /// Last value
    /// </summary>
    int _v = 0;
    
    public string Next()
    {
        _v++;
        return _v.ToString();
    }
}