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
        return await _cardService.ByKey(key);
    }

    public Task<CharacterCardTemplate> GetCharacter(string key)
    {
        // TODO
        throw new NotImplementedException();
    }

    public Task<List<string>> GetCharacterKeys()
    {
        // TODO
        throw new NotImplementedException();
    }

    public async Task<List<string>> GetKeys()
    {
        return (await _cardService.GetKeys()).ToList();
    }

    public Task<MonsterCardTemplate> GetMonster(string key)
    {
        // TODO
        throw new NotImplementedException();
    }

    public Task<List<string>> GetMonsterKeys()
    {
        // TODO
        throw new NotImplementedException();
    }

    public Task<CharacterCardTemplate> GetRandomCharacter(Random rng, List<string> keys)
    {
        // TODO
        throw new NotImplementedException();
    }
}
