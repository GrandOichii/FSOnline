namespace FSLogsCollector.Services;

public interface ILoggedMessageProcessor {
    public Task Process(byte[] message);
}