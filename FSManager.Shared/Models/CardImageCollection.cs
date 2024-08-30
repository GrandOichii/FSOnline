namespace FSManager.Shared.Models;

public class CardImageCollection {
    public required string Key { get; set; }

    public required List<CardImage> Images { get; set; }
}