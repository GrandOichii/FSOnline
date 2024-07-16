using System.Text.Json;
using FSCore.Cards;
using FSCore.Matches.Cards;

namespace FSTester;

public class FileCardMasterCardsData {
    public required List<string> Characters { get; set; }
    public required List<string> StartingItems { get; set; }
    public required List<string> Loot { get; set; }
}

public class FileCardMasterData {
    public required FileCardMasterCardsData Cards { get; set; }
}

public class FileCardMaster : ICardMaster
{
    private readonly Dictionary<string, CardTemplate> _index = new();
    private readonly Dictionary<string, CharacterCardTemplate> _characterIndex = new();

    private static void AddTo(string dir, List<string> paths, Dictionary<string, CardTemplate> index) {
        foreach (var c in paths) {
            var dataPath = Path.Join(dir, c);
            var card = JsonSerializer.Deserialize<CardTemplate>(File.ReadAllText(dataPath + ".json"))
                ?? throw new Exception($"failed to deserialize json in {dataPath}")
            ;
            var script = File.ReadAllText(dataPath + ".lua");
            card.Script = script;
            index.Add(card.Name, card);
        }
    }

    private static void AddCharactersTo(string dir, List<string> paths, Dictionary<string, CharacterCardTemplate> index) {
        foreach (var c in paths) {
            var dataPath = Path.Join(dir, c);
            var card = JsonSerializer.Deserialize<CharacterCardTemplate>(File.ReadAllText(dataPath + ".json"))
                ?? throw new Exception($"failed to deserialize json in {dataPath}")
            ;
            var script = File.ReadAllText(dataPath + ".lua");
            card.Script = script;
            index.Add(card.Name, card);
        }
    }

    public void Load(string dir) {
        var manifestPath = Path.Join(dir, "manifest.json");
        var data = JsonSerializer.Deserialize<FileCardMasterData>(File.ReadAllText(manifestPath))
            ?? throw new Exception($"failed to deserialize json in {manifestPath}")
        ;

        AddTo(dir, data.Cards.Loot, _index);
        AddTo(dir, data.Cards.StartingItems, _index);
        AddCharactersTo(dir, data.Cards.Characters, _characterIndex);
    }

    public Task<CardTemplate> Get(string key)
    {
        return Task.FromResult(_index[key]);
    }

    public Task<CharacterCardTemplate> GetCharacter(string key)
    {
        return Task.FromResult(_characterIndex[key]);
    }
}
