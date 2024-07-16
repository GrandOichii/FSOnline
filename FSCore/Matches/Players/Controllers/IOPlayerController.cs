using System.Text.Json;

namespace FSCore.Matches.Players.Controllers;

public interface IIOHandler {
    public Task<string> Read();
    public Task Write(string msg);
    public Task Close();
}

public class IOPlayerController : IPlayerController
{
    private readonly IIOHandler _handler;

    public IOPlayerController(IIOHandler handler) {
        _handler = handler;
    }

    /// <summary>
    /// Write new personalized data
    /// </summary>
    /// <param name="data">New match data</param>
    private async Task WriteData(PersonalizedData data) {
        var json = JsonSerializer.Serialize(data);
        await _handler.Write(json);
    }

    /// <summary>
    /// Turns the provided list to the args value
    /// </summary>
    /// <param name="list">List of values</param>
    /// <returns>Args value</returns>
    private static Dictionary<string, object> ToArgs<T>(IEnumerable<T> options) where T : class {
        return options.Select(
            (o, i) => new {o, i}
        ).ToDictionary(
            e => e.i.ToString(), 
            e => (object)e.o // TODO this looks bad
        );
    }

    public async Task Update(Match match, int playerIdx)
    {
        await WriteData(new(match, playerIdx) {
            Request = "Update",
            Hint = "",
            Args = new(),
        });
    }

    public async Task Setup(Match match, int playerI)
    {
        var info = new MatchInfo(match, playerI);
        var data = JsonSerializer.Serialize(info);
        await _handler.Write(data);
    }

    public async Task CleanUp(Match match, int playerIdx)
    {
        await _handler.Close();
    }

    public async Task<string> PromptAction(Match match, int playerIdx, IEnumerable<string> options)
    {
        await WriteData(new(match, playerIdx) {
            Request = "PromptAction",
            Hint = "",
            Args = ToArgs(options),
        });
       
        throw new NotImplementedException();
    }
}