using FSManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FSManager.Repositories;

public class CardRepository : DbContext, ICardRepository
{
    public DbSet<CardModel> Cards { get; set; }
    public DbSet<CardImageCollection> ImageCollections { get; set; }

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

        // card image collections
        var imageCollectionTable = modelBuilder.Entity<CardImageCollection>()
            .ToTable("CardImageCollections");
        imageCollectionTable.HasKey(col => col.Key);

        // card images
        var imageTable = modelBuilder.Entity<CardImage>()
            .ToTable("CardImages");
        imageTable.HasKey(col => col.ID);
        imageTable.Property(col => col.ID).ValueGeneratedOnAdd();

        // card collections
        var collectionsTable = modelBuilder.Entity<CardCollection>()
            .ToTable("CardCollections");
        collectionsTable.HasKey(col => col.Key);

        // relations
        cardsTable
            .HasMany(card => card.Images)
            .WithOne(img => img.Card);
        
        imageCollectionTable
            .HasMany(col => col.Images)
            .WithOne(img => img.Collection);

        collectionsTable
            .HasMany(col => col.Cards)
            .WithOne(card => card.Collection);
    }

    public Task<string> GetDefaultCardImageCollectionKey() {
        var result = Database
            .SqlQuery<string>($"SELECT dbo.getDefaultCardImageCollectionKey()")
            .ToList();
        return Task.FromResult(
            result[0]
        );
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
            .Include(c => c.Images)
                .ThenInclude(img => img.Collection)
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
}