namespace FSCore.Matches.Cards;

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
    /// Fetch a random character card
    /// </summary>
    /// <param name="rng">Random number generator</param>
    /// <returns>Random character card template</returns>
    public Task<CharacterCardTemplate> GetRandomCharacter(Random rng);
}