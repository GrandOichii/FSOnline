using System.Diagnostics;
using System.Text.Json;
using FSCore.Cards;
using FSCore.Matches.Cards;

namespace FSCore.Cards.CardMasters;

public class FileCardMasterCardsData {
    public required List<string> Characters { get; set; }
    public required List<string> StartingItems { get; set; }
    public required List<string> Treasures { get; set; }
    public required List<string> Loot { get; set; }
    public required List<string> BonusSouls { get; set; }
    public required List<string> Rooms { get; set; }
    public required List<string> Monsters { get; set; }
    public required List<string> Events { get; set; }
    public required List<string> Curses { get; set; }
}

public class FileCardMasterData {
    public required FileCardMasterCardsData Cards { get; set; }
}

public class FileCardMaster : ICardMaster
{
    private readonly Dictionary<string, CardTemplate> _index = [];

    private static void AddTo(string dir, List<string> paths, Dictionary<string, CardTemplate> index) {
        foreach (var c in paths) {
            var dataPath = Path.Join(dir, c);
            var card = JsonSerializer.Deserialize<CardTemplate>(File.ReadAllText(dataPath + ".json"))
                ?? throw new Exception($"failed to deserialize card template json in {dataPath}")
            ;
            var script = File.ReadAllText(dataPath + ".lua");
            card.Script = script;
            index.Add(card.Key, card);
            // System.Console.WriteLine("Loaded card " + card.Key);
        }
    }

    public void Load(string dir) {
        var manifestPath = Path.Join(dir, "manifest.json");
        var data = JsonSerializer.Deserialize<FileCardMasterData>(File.ReadAllText(manifestPath))
            ?? throw new Exception($"failed to deserialize json in {manifestPath}")
        ;

        AddTo(dir, data.Cards.Loot, _index);
        AddTo(dir, data.Cards.StartingItems, _index);
        AddTo(dir, data.Cards.Treasures, _index);
        AddTo(dir, data.Cards.BonusSouls, _index);
        AddTo(dir, data.Cards.Rooms, _index);
        AddTo(dir, data.Cards.Events, _index);
        AddTo(dir, data.Cards.Curses, _index);
        AddTo(dir, data.Cards.Monsters, _index);
        AddTo(dir, data.Cards.Characters, _index);
    }

    public Task<CardTemplate> Get(string key)
    {
        return Task.FromResult(_index[key]);
    }

    public Task<List<string>> GetKeys()
    {
        return Task.FromResult(
            _index.Keys.ToList()
        );
    }

    public Task<List<string>> GetKeysOfType(string type) {
        return Task.FromResult(
            _index.Values
                .Where(c => c.Type == type)
                .Select(c => c.Key)
                .ToList()
        );
    }
}
