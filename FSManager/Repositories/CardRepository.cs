using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FSManager.Repositories;

public class CardRepository : DbContext, ICardRepository
{
    public DbSet<CardModel> Cards { get; set; }

    public CardRepository(DbContextOptions<CardRepository> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // cards
        var cardsTable = modelBuilder.Entity<CardModel>()
            .ToTable("Cards");
        cardsTable.HasKey(card => card.Key);

        // card collections
        var collectionsTable = modelBuilder.Entity<CardCollection>()
            .ToTable("CardCollections");
        collectionsTable.HasKey(col => col.Key);

        // card relations
        var cardRelationsTable = modelBuilder.Entity<CardRelation>()
            .ToTable("CardRelations");
        cardRelationsTable.HasKey(rel => rel.ID);
        cardRelationsTable
            .Property(rel => rel.ID)
            .ValueGeneratedOnAdd();

        // relations
        collectionsTable
            .HasMany(col => col.Cards)
            .WithOne(card => card.Collection)
        ;

        cardsTable
            .HasMany(card => card.Relations)
            .WithOne(rel => rel.RelatedTo);

        cardsTable
            .HasMany(card => card.RelatedTo)
            .WithOne(rel => rel.RelatedCard);
    }

    public async Task CreateCard(
        string key,
        string name,
        string type,
        int health,
        int attack,
        int evasion,
        string text,
        string script,
        int soulValue,
        string collectionKey,
        string defaultImageSrc
    ) {
        Database.ExecuteSql($"EXEC createCard {key}, {name}, {type}, {health}, {attack}, {evasion}, {text}, {script}, {soulValue}, {collectionKey}, {defaultImageSrc}");
        await SaveChangesAsync();
    }

    private IQueryable<CardModel> FetchCards() {
        return Cards
            .Include(c => c.Collection)
            .Include(c => c.Relations)
                .ThenInclude(rel => rel.RelatedCard)
            .Include(c => c.RelatedTo)
                .ThenInclude(rel => rel.RelatedTo)
        ;
    } 

    public Task<IEnumerable<CardModel>> AllCards() {
        return Task.FromResult(
            FetchCards()
                .AsEnumerable()
        );
    }

    public Task<CardModel?> ByKey(string key) {
        return Task.FromResult(
            FetchCards()
                .FirstOrDefault(card => card.Key == key)
        );
    }

    public async Task<bool> RemoveCard(string key) {
        var card = await ByKey(key);

        if (card is null)
            return false;

        Cards.Remove(card);
        await SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<CardModel>> GetCards() {
        return FetchCards();
    }

    public async Task SaveRelation(CardRelation relation)
    {
        relation.RelatedTo.Relations.Add(relation);
        await SaveChangesAsync();
    }

    public async Task DeleteRelation(CardRelation relation) {
        Entry(relation).State = EntityState.Deleted;
        await SaveChangesAsync();
    }

    public async Task UpdateRelationType(CardRelation relation, CardRelationType relationType)
    {
        relation.RelationType = relationType;
        await SaveChangesAsync();
    }
}