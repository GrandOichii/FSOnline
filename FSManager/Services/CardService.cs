
using Microsoft.EntityFrameworkCore;

namespace FSManager.Services;

public class CardService : ICardService
{
    private readonly CardsContext _cards;

    public CardService(CardsContext cards) {
        _cards = cards;
    }

    public async Task<IEnumerable<GetCard>> All(string? cardImageCollection = null)
    {
        cardImageCollection ??= _cards.GetDefaultCardImageCollectionKey();

        var cards = _cards.Cards
            .Include(c => c.Images)
            .ThenInclude(img => img.Collection);
        
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

        var result = _cards.Cards.Include(c => c.Images).ThenInclude(img => img.Collection).FirstOrDefault(c => c.Key == card.Key)
            ?? throw new Exception($"Created card with key {card.Key}, but failed to fetch it");
        var colKey = _cards.GetDefaultCardImageCollectionKey();

        return new GetCard {
            Key = result.Key,
            Name = result.Name,
            Text = result.Text,
            ImageUrl = result.Images.First(img => img.Collection.Key == colKey).Source
        };
    }
}
