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

    public Task<List<string>> GetKeys();
    public Task<List<string>> GetKeysOfType(string type);
    public async Task<List<string>> GetMonsterKeys() => await GetKeysOfType("Monster");
}