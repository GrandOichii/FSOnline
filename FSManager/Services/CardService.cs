
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
}
