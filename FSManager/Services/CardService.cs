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
        await _cards.CreateCard(card.Key, card.Name, card.Text, card.CollectionKey, card.DefaultImageSrc);

        var result = await _cards.ByKey(card.Key)
            ?? throw new Exception($"Created card with key {card.Key}, but failed to fetch it");
        var colKey = await _cards.GetDefaultCardImageCollectionKey();

        return MapToGetCard(result, colKey);
    }

    public async Task Delete(string key) {
        var deleted = await _cards.RemoveCard(key);
        if (deleted) return;

        throw new FailedToDeleteCardException($"Failed to delete card with key {key}");
    }

    public GetCard MapToGetCard(CardModel card, string imageCollectionKey) {
        var result = _mapper.Map<GetCard>(card);
        result.ImageUrl = card.Images.First(img => img.Collection.Key == imageCollectionKey).Source;
        return result;
    } 
}
