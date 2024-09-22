using System.Text.Json.Serialization;

namespace FSLogsCollector;
public class LoggedMessageProperties {
    public required string SourceContext { get; set; }
}

public class LoggedMessage {
    public required DateTime Timestamp { get; set; }
    public required string MessageTemplate { get; set; }
    public required string Level { get; set; }
    public required LoggedMessageProperties Properties { get; set; }
}