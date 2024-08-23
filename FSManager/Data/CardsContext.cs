using FSManager.Models;
using Microsoft.EntityFrameworkCore;

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
        imageCollectionTable.HasKey(col => col.ID);
        imageCollectionTable.Property(col => col.ID).ValueGeneratedOnAdd();

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
        
        // procedures
    }
}