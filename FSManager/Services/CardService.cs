
using Microsoft.EntityFrameworkCore;

namespace FSManager.Services;

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
            ImageUrl = c.Images.First(img => img.Collection.Key == cardImageCollection).Source
        });
    }

    public async Task<GetCard> Create(PostCard card)
    {
        await _cards.CreateCard(card.Key, card.Name, card.Text, card.DefaultImageSrc);

        var result = await _cards.ByKey(card.Key)
            ?? throw new Exception($"Created card with key {card.Key}, but failed to fetch it");
        var colKey = await _cards.GetDefaultCardImageCollectionKey();

        return new GetCard {
            Key = result.Key,
            Name = result.Name,
            Text = result.Text,
            ImageUrl = result.Images.First(img => img.Collection.Key == colKey).Source
        };
    }
}
