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
}

public class FileCardMasterData {
    public required FileCardMasterCardsData Cards { get; set; }
}

public class FileCardMaster : ICardMaster
{
    private readonly Dictionary<string, CardTemplate> _index = [];
    private readonly Dictionary<string, CharacterCardTemplate> _characterIndex = [];
    private readonly Dictionary<string, MonsterCardTemplate> _monsterIndex = [];

    private static void AddTo(string dir, List<string> paths, Dictionary<string, CardTemplate> index) {
        foreach (var c in paths) {
            var dataPath = Path.Join(dir, c);
            var card = JsonSerializer.Deserialize<CardTemplate>(File.ReadAllText(dataPath + ".json"))
                ?? throw new Exception($"failed to deserialize card template json in {dataPath}")
            ;
            var script = File.ReadAllText(dataPath + ".lua");
            card.Script = script;
            index.Add(card.Key, card);
            System.Console.WriteLine("Loaded card " + card.Key);
        }
    }

    private static void AddCharactersTo(string dir, List<string> paths, Dictionary<string, CharacterCardTemplate> index) {
        foreach (var c in paths) {
            var dataPath = Path.Join(dir, c);
            var card = JsonSerializer.Deserialize<CharacterCardTemplate>(File.ReadAllText(dataPath + ".json"))
                ?? throw new Exception($"failed to deserialize character card template json in {dataPath}")
            ;
            var script = File.ReadAllText(dataPath + ".lua");
            card.Script = script;
            index.Add(card.Key, card);
            System.Console.WriteLine("Loaded character card " + card.Key);
        }
    }

    private static void AddMonstersTo(string dir, List<string> paths, Dictionary<string, MonsterCardTemplate> index) {
        foreach (var c in paths) {
            var dataPath = Path.Join(dir, c);
            var card = JsonSerializer.Deserialize<MonsterCardTemplate>(File.ReadAllText(dataPath + ".json"))
                ?? throw new Exception($"failed to deserialize monster card template json in {dataPath}")
            ;
            var script = File.ReadAllText(dataPath + ".lua");
            card.Script = script;
            index.Add(card.Key, card);
            System.Console.WriteLine("Loaded monster card " + card.Key);
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
        AddCharactersTo(dir, data.Cards.Characters, _characterIndex);
        AddMonstersTo(dir, data.Cards.Monsters, _monsterIndex);
    }

    public Task<CardTemplate> Get(string key)
    {
        return Task.FromResult(_index[key]);
    }

    public Task<CharacterCardTemplate> GetCharacter(string key)
    {
        return Task.FromResult(_characterIndex[key]);
    }

    public Task<MonsterCardTemplate> GetMonster(string key)
    {
        return Task.FromResult(_monsterIndex[key]);
    }

    public Task<CharacterCardTemplate> GetRandomCharacter(Random rng, List<string> characters)
    {
        return Task.FromResult(
            _characterIndex[characters[rng.Next() % characters.Count]]
        );
    }

    public Task<List<string>> GetKeys()
    {
        return Task.FromResult(
            _index.Keys.ToList()
        );
    }

    public Task<List<string>> GetCharacterKeys()
    {
        return Task.FromResult(
            _characterIndex.Keys.ToList()
        );
    }

    public Task<List<string>> GetMonsterKeys()
    {
        return Task.FromResult(
            _monsterIndex.Keys.ToList()
        );
    }
}
