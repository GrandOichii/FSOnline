namespace FSCore.Matches.Cards;

public interface ICardMaster {
    public Task<CardTemplate> Get(string name);
}