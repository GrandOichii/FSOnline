namespace FSCore.Matches;

public class MatchException : FSCoreException {
    public MatchException() : base() {}
    public MatchException(string msg) : base(msg) {}
    public MatchException(string message, Exception inner) : base(message, inner) { }
}