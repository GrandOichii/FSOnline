namespace FSMatchManager.Services;

[System.Serializable]
public class FailedToCreateMatchException : Exception
{
    public FailedToCreateMatchException() { }
    public FailedToCreateMatchException(string message) : base(message) { }
    public FailedToCreateMatchException(string message, Exception inner) : base(message, inner) { }
}

public interface IMatchService {
    public Task<List<MatchProcess>> All();
    public Task<MatchProcess> Create(CreateMatch config);
}