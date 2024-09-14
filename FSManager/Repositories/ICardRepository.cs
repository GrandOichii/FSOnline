namespace FSManager.Repositories;

public interface ICardRepository {
    public Task CreateCard(
        string key,
        string name,
        string type,
        int health,
        int attack,
        int evasion,
        string text,
        string script,
        int soulValue,
        string rewardsText,
        string collectionKey,
        string imageUrl
    );
    public Task UpdateCard(CardModel existing, CardModel replacement);
    public Task<CardModel?> ByKey(string key);
    public Task<bool> RemoveCard(string key);
    public Task<IQueryable<CardModel>> GetCards();
    
    public Task SaveRelation(CardRelation relation);
    public Task DeleteRelation(CardRelation relation);
    public Task UpdateRelationType(CardRelation relation, CardRelationType relationType);
}