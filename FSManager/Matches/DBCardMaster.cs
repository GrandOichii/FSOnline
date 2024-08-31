using FSCore.Cards.CardMasters;

namespace FSManager.Matches;

public class DBCardMaster : ICardMaster
{
    private readonly ICardService _cardService;
    
    public DBCardMaster(ICardService cardService){
        _cardService = cardService;
    }

    public async Task<CardTemplate> Get(string key)
    {
        System.Console.WriteLine($"GETTING {key}");
        var result = await _cardService.ByKey(key);
        System.Console.WriteLine($"GOT {result.Key}");
        return result;
    }

    public async Task<List<string>> GetKeys()
    {
        return (await _cardService.GetKeys())
            .ToList();
    }

    public async Task<List<string>> GetKeysOfType(string type)
    {
        return (await _cardService.OfType(type))
            .Select(c => c.Key)
            .ToList();
    }
}
