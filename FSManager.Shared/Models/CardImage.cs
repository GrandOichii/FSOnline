namespace FSManager.Shared.Models;

public class CardImage {
    public required int ID { get; set; }
    public required string Source { get; set; }

    public required CardModel Card { get; set; }
    public required CardImageCollection Collection { get; set; }
}