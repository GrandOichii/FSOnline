namespace FSCore.Cards.CardMasters;

/// <summary>
/// Is thrown when an ICardMaster object fails to fetch a card template
/// </summary>
[Serializable]
public class ICardMasterException : FSCoreException
{
    public ICardMasterException() { }
    public ICardMasterException(string message) : base(message) { }
    public ICardMasterException(string message, System.Exception inner) : base(message, inner) { }
}

/// <summary>
/// Card master, used for fetching cards
/// </summary>
public interface ICardMaster {
    /// <summary>
    /// Fetch a card
    /// </summary>
    /// <param name="key">Card key</param>
    /// <returns>Card template associated with the given key</returns>
    public Task<CardTemplate> Get(string key);
    /// <summary>
    /// Fetch a character card
    /// </summary>
    /// <param name="key">Character card key</param>
    /// <returns>Character card template associated with the given key</returns>
    public Task<CharacterCardTemplate> GetCharacter(string key);
    /// <summary>
    /// Fetch a monster card
    /// </summary>
    /// <param name="key">Monster card key</param>
    /// <returns>Monster card template associated with the given key</returns>
    public Task<MonsterCardTemplate> GetMonster(string key);

    /// <summary>
    /// Fetch a random character card
    /// </summary>
    /// <param name="rng">Random number generator</param>
    /// <param name="keys">List of available character keys</param>
    /// <returns>Random character card template</returns>
    public Task<CharacterCardTemplate> GetRandomCharacter(Random rng, List<string> keys);

    public Task<List<string>> GetKeys();
    public Task<List<string>> GetCharacterKeys();
    public Task<List<string>> GetMonsterKeys();
}