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

        // relations
        collectionsTable
            .HasMany(col => col.Cards)
            .WithOne(card => card.Collection);
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
}