using AutoMapper;
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
public class CardNotFoundException : CardServiceException
{
    public CardNotFoundException() { }
    public CardNotFoundException(string message) : base(message) { }
    public CardNotFoundException(string message, System.Exception inner) : base(message, inner) { }
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
    private readonly IMapper _mapper;

    public CardService(ICardRepository cards, IMapper mapper) {
        _cards = cards;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GetCard>> All(string? cardImageCollection = null)
    {
        cardImageCollection ??= await _cards.GetDefaultCardImageCollectionKey();
        var cards = await _cards.AllCards();

        return cards.Select(
            c => MapToGetCard(c, cardImageCollection)
        );
    }

    public async Task<GetCard> Create(PostCard card)
    {
        await _cards.CreateCard(
            card.Key,
            card.Name,
            card.Type,
            card.Attack,
            card.Health,
            card.Evasion,
            card.Text,
            card.Script,
            card.SoulValue,
            card.CollectionKey,
            card.DefaultImageSrc
        );

        return await ByKey(card.Key);
    }

    public async Task Delete(string key) {
        if (await _cards.ByKey(key) is null)
            throw new CardNotFoundException($"Card with key {key} not found");

        try {
            var deleted = await _cards.RemoveCard(key);
            if (deleted) return;
        } catch (Exception ex) {
            throw new FailedToDeleteCardException($"Failed to delete card with key {key}", ex);
        }

        throw new FailedToDeleteCardException($"Failed to delete card with key {key}");

    }

    public async Task<GetCard> ByKey(string key) {
        var result = await _cards.ByKey(key)
            ?? throw new CardNotFoundException($"Created card with key {key}, but failed to fetch it");
        var colKey = await _cards.GetDefaultCardImageCollectionKey();

        return MapToGetCard(result, colKey);
    }

    private GetCard MapToGetCard(CardModel card, string imageCollectionKey) {
        return _mapper.Map<GetCard>(
            card,
            o => o.Items["ICK"] = imageCollectionKey
        );
    }

    public async Task<IEnumerable<string>> GetKeys() {
        return (await _cards.GetCards())
            .Select(card => card.Key);
    }

    public async Task<IEnumerable<GetCard>> FromCollection(string collectionKey) {
        var colKey = await _cards.GetDefaultCardImageCollectionKey();

        return (await _cards.GetCards())
            .Where(c => c.Collection.Key == collectionKey)
            .Select(c => MapToGetCard(c, colKey));
    }
}
