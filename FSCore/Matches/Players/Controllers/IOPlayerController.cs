using System.Text.Json;

namespace FSCore.Matches.Players.Controllers;

/// <summary>
/// Player input and output handler
/// </summary>
public interface IIOHandler {
    /// <summary>
    /// Reads a string
    /// </summary>
    /// <returns>The read string</returns>
    public Task<string> Read();
    /// <summary>
    /// Writes a string
    /// </summary>
    /// <param name="msg">Message</param>
    public Task Write(string msg);
    /// <summary>
    /// Closes the IIOHandler
    /// </summary>
    public Task Close();
}

/// <summary>
/// Input-output based player controller
/// </summary>
public class IOPlayerController : IPlayerController
{
    /// <summary>
    /// Input and output handler
    /// </summary>
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
    private static Dictionary<string, object> ToArgs<T>(IEnumerable<T> options) where T : notnull {
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

        return await _handler.Read();
    }

    public async Task<string> ChooseString(Match match, int playerIdx, List<string> options, string hint)
    {
        await WriteData(new(match, playerIdx) {
            Request = "ChooseString",
            Hint = hint,
            Args = ToArgs(options),
        });

        return await _handler.Read();
     }

    public async Task<int> ChoosePlayer(Match match, int playerIdx, List<int> options, string hint)
    {
        await WriteData(new(match, playerIdx) {
            Request = "ChooseString",
            Hint = hint,
            Args = ToArgs(options),
        });

        return int.Parse(await _handler.Read());
     }
}