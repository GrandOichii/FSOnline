using AutoMapper;
using Microsoft.Extensions.Options;

namespace FSManager.Services;

public class CardService : ICardService
{
    private readonly ICardRepository _cards;
    private readonly ICollectionRepository _collections;
    private readonly IMapper _mapper;
    private readonly IOptions<CardSettings> _settings;

    public CardService(ICardRepository cards, ICollectionRepository collections, IMapper mapper, IOptions<CardSettings> settings)
    {
        _cards = cards;
        _collections = collections;
        _settings = settings;
        _mapper = mapper;
    }

    private CardsPage ToCardsPage(IQueryable<CardModel> cards, int page) {
        return new CardsPage
        {
            Cards = Paginate(cards, page)
                .Select(_mapper.Map<GetCard>)
                .ToList(),
            Page = page,
            PageCount = PageCount(cards),
        };
    }

    public async Task<CardsPage> All(int page)
    {
        var cards = await _cards.GetCards();

        return ToCardsPage(cards, page);
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

    public async Task Delete(string key)
    {
        if (await _cards.ByKey(key) is null)
            throw new CardNotFoundException(key);

        try
        {
            var deleted = await _cards.RemoveCard(key);
            if (deleted) return;
        }
        catch (Exception ex)
        {
            throw new FailedToDeleteCardException($"Failed to delete card with key {key}", ex);
        }

        throw new FailedToDeleteCardException($"Failed to delete card with key {key}");

    }

    public async Task<GetCardWithRelations> ByKey(string key)
    {
        var result = await _cards.ByKey(key)
            ?? throw new CardNotFoundException(key);

        return MapToGetCard(result);
    }

    private GetCardWithRelations MapToGetCard(CardModel card)
    {
        return _mapper.Map<GetCardWithRelations>(
            card
        );
    }

    public async Task<IEnumerable<string>> GetKeys()
    {
        return (await _cards.GetCards())
            .Select(card => card.Key);
    }

    public async Task<CardsPage> FromCollection(string collectionKey, int page)
    {
        var cards = (await _cards.GetCards()).Where(c => c.Collection.Key == collectionKey);
        return ToCardsPage(cards, page);
    }

    public async Task<IEnumerable<GetCard>> OfType(string type)
    {
        return (await _cards.GetCards())
            .Where(c => c.Type == type)
            .Select(c => MapToGetCard(c));
    }

    public async Task CreateRelation(string cardKey, string relatedCardKey, CardRelationType relationType)
    {
        if (cardKey == relatedCardKey)
            throw new RelationWithSelfException($"Tried to create a relation for card {cardKey} with itself");

        var card = await _cards.ByKey(cardKey)
            ?? throw new CardNotFoundException(cardKey);
        var relatedCard = await _cards.ByKey(relatedCardKey)
            ?? throw new CardNotFoundException(relatedCardKey);

        if (GetRelation(card, relatedCard) is not null)
            throw new RelationAlreadyExistsException(cardKey, relatedCardKey);

        var relation = new CardRelation()
        {
            RelatedTo = card,
            RelatedCard = relatedCard,
            RelationType = relationType
        };

        await _cards.SaveRelation(relation);
    }

    private static CardRelation? GetRelation(CardModel card1, CardModel card2)
    {
        return
            card1.Relations.FirstOrDefault(rel => rel.RelatedCard.Key == card2.Key) ??
            card1.RelatedTo.FirstOrDefault(rel => rel.RelatedTo.Key == card2.Key);
    }

    public async Task DeleteRelation(string cardKey, string relatedCardKey)
    {
        if (cardKey == relatedCardKey)
            throw new RelationWithSelfException($"Tried to delete a relation for card {cardKey} with itself");

        var card = await _cards.ByKey(cardKey)
            ?? throw new CardNotFoundException(cardKey);
        var relatedCard = await _cards.ByKey(relatedCardKey)
            ?? throw new CardNotFoundException(relatedCardKey);

        var relation = GetRelation(card, relatedCard)
            ?? throw new RelationNotFoundException(cardKey, relatedCardKey)
        ;

        await _cards.DeleteRelation(relation);
    }

    public async Task EditRelationType(string cardKey, string relatedCardKey, CardRelationType relationType)
    {
        if (cardKey == relatedCardKey)
            throw new RelationWithSelfException($"Tried to edit a relation for card {cardKey} with itself");

        var card = await _cards.ByKey(cardKey)
            ?? throw new CardNotFoundException(cardKey);
        var relatedCard = await _cards.ByKey(relatedCardKey)
            ?? throw new CardNotFoundException(relatedCardKey);

        var relation = GetRelation(card, relatedCard)
            ?? throw new RelationNotFoundException(cardKey, relatedCardKey)
        ;

        await _cards.UpdateRelationType(relation, relationType);
    }

    public async Task<IEnumerable<GetCollection>> GetCollections()
    {
        return (await _collections.All()).Select(_mapper.Map<GetCollection>);
    }

    private IQueryable<CardModel> Paginate(IQueryable<CardModel> query, int page)
    {
        return query
            .Skip(page * _settings.Value.CardsPerPage)
            .Take(_settings.Value.CardsPerPage);
    }

    private int PageCount(IQueryable<CardModel> query)
    {
        return (int)Math.Ceiling(query.Count() * 1.0 / _settings.Value.CardsPerPage);
    }

    public async Task<CardsPage> Filter(CardFilter filter, int page)
    {
        var query = await _cards.GetCards();
        var cards = filter.Modify(query);

        return ToCardsPage(cards, page);
    }
}
