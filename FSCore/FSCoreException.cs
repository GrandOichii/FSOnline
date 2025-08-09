namespace FSCore;

/// <summary>
/// Core exception type, all FSCore exceptions inherit from FSCoreException
/// </summary>
[Serializable]
public class FSCoreException : Exception
{
    public FSCoreException() { }
    public FSCoreException(string message) : base(message) { }
    public FSCoreException(string message, Exception inner) : base(message, inner) { }
}