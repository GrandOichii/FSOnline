using FSManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FSManager.Data;

public class CardsContext : DbContext
{
    public DbSet<CardModel> Cards { get; set; }
    public DbSet<CardImageCollection> ImageCollections { get; set; }

    public CardsContext(DbContextOptions<CardsContext> options)
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


        // relations
        cardsTable
            .HasMany(card => card.Images)
            .WithOne(img => img.Card);
        
        imageCollectionTable
            .HasMany(col => col.Images)
            .WithOne(img => img.Collection);
    }

    public string GetDefaultCardImageCollectionKey() {
        var result = Database
            .SqlQuery<string>($"SELECT dbo.getDefaultCardImageCollectionKey()")
            .ToList();
        return result[0];
    }

    public async Task CreateCard(
        string key,
        string name,
        string text,string defaultImageSrc
    ) {
        Database.ExecuteSql($"EXEC createCard {key}, {name}, {text}, {defaultImageSrc}");
        await SaveChangesAsync();
    }

    private IQueryable<CardModel> FetchCards() {
        return Cards
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

}