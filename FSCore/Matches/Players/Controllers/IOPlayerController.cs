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
 
    private async Task WriteData(PersonalizedData data) {
        var json = JsonSerializer.Serialize(data);
        await _handler.Write(json);
    }

    private static Dictionary<string, string> OptionsToDict<T>(List<T> options) {
        return options.Select((o, i) => new {o, i}).ToDictionary(e => e.i.ToString(), e => e.o!.ToString()!);
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
}