using Microsoft.EntityFrameworkCore;

namespace FSManager.Services;

[System.Serializable]
public class CardServiceException : System.Exception
{
    public CardServiceException() { }
    public CardServiceException(string message) : base(message) { }
    public CardServiceException(string message, System.Exception inner) : base(message, inner) { }
}

[System.Serializable]
public class FailedToDeleteCardException : CardServiceException
{
    public FailedToDeleteCardException() { }
    public FailedToDeleteCardException(string message) : base(message) { }
    public FailedToDeleteCardException(string message, System.Exception inner) : base(message, inner) { }
}

public class CardService : ICardService
{
    private readonly ICardRepository _cards;

    public CardService(ICardRepository cards) {
        _cards = cards;
    }

    public async Task<IEnumerable<GetCard>> All(string? cardImageCollection = null)
    {
        cardImageCollection ??= await _cards.GetDefaultCardImageCollectionKey();
        var cards = await _cards.AllCards();

        return cards.Select(c => new GetCard {
            Key = c.Key,
            Name = c.Name,
            Text = c.Text,
            Collection = c.Collection.Key,
            ImageUrl = c.Images.First(img => img.Collection.Key == cardImageCollection).Source
        });
    }

    public async Task<GetCard> Create(PostCard card)
    {
        await _cards.CreateCard(card.Key, card.Name, card.Text, card.CollectionKey, card.DefaultImageSrc);

        var result = await _cards.ByKey(card.Key)
            ?? throw new Exception($"Created card with key {card.Key}, but failed to fetch it");
        var colKey = await _cards.GetDefaultCardImageCollectionKey();

        return new GetCard {
            Key = result.Key,
            Name = result.Name,
            Text = result.Text,
            Collection = result.Collection.Key,
            ImageUrl = result.Images.First(img => img.Collection.Key == colKey).Source
        };
    }

    public async Task Delete(string key) {
        var deleted = await _cards.RemoveCard(key);
        if (deleted) return;

        throw new FailedToDeleteCardException($"Failed to delete card with key {key}");
    }
}
