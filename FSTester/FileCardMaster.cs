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

// public class FileCardMaster : ICardMaster
// {
//     private readonly Dictionary<string, CardTemplate> _index = new();
//     private readonly Dictionary<string, CharacterCardTemplate> _characterIndex = new();

//     public void Load(string dir) {
//         var manifestPath = Path.Join(dir, "manifest.json");
//         var data = JsonSerializer.Deserialize<FileCardMasterData>(File.ReadAllText(manifestPath))
//             ?? throw new Exception($"failed to deserialize json in {manifestPath}")
//         ;
//         foreach (var c in data.Cards) {
//             var dataPath = Path.Join(dir, c);
//             var card = JsonSerializer.Deserialize<CardTemplate>(File.ReadAllText(dataPath + ".json"))
//                 ?? throw new Exception($"failed to deserialize json in {dataPath}")
//             ;
//             var script = File.ReadAllText(dataPath + ".lua");
//             card.Script = script;
//             _index.Add(card.Name, card);
//         }
//         foreach (var c in data.Heroes) {
//             var dataPath = Path.Join(dir, c);
//             var card = JsonSerializer.Deserialize<HeroTemplate>(File.ReadAllText(dataPath + ".json"))
//                 ?? throw new Exception($"failed to deserialize json in {dataPath}")
//             ;
//             var script = File.ReadAllText(dataPath + ".lua");
//             card.Script = script;
//             _characterIndex.Add(card.Name, card);
//         }
//     }

//     public Task<CardTemplate> Get(string key)
//     {
//         return Task.FromResult(_index[key]);
//     }

//     public Task<CharacterCardTemplate> GetHero(string key)
//     {
//         return Task.FromResult(_characterIndex[key]);
//     }
// }
