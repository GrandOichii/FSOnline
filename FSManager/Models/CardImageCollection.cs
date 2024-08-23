namespace FSManager.Models;

public class CardImageCollection {
    public required int ID { get; set; }
    public required string Name { get; set; }

    public required List<CardImage> Images { get; set; }
}