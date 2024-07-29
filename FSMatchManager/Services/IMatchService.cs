namespace FSMatchManager.Services;

[Serializable]
public class MatchNotFoundException : Exception
{
    public MatchNotFoundException() { }
    public MatchNotFoundException(string message) : base(message) { }
    public MatchNotFoundException(string message, Exception inner) : base(message, inner) { }
}

[Serializable]
public class FailedToCreateMatchException : Exception
{
    public FailedToCreateMatchException() { }
    public FailedToCreateMatchException(string message) : base(message) { }
    public FailedToCreateMatchException(string message, Exception inner) : base(message, inner) { }
}

public interface IMatchService {
    public Task<List<MatchProcess>> All();
    public Task<MatchProcess> WebSocketCreate(WebSocketManager wsManager);
    public Task WebSocketConnect(string matchId, WebSocketManager wsManager);
}