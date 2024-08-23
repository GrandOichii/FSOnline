using FSManager.Models;
using Microsoft.EntityFrameworkCore;

namespace FSManager.Data;

public class CardsContext : DbContext
{
    public CardsContext(DbContextOptions<CardsContext> options)
        : base(options)
    {
        
    }

    public DbSet<CardModel> Cards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CardModel>()
            .ToTable("Cards").HasKey(card => card.Key);
    }
}