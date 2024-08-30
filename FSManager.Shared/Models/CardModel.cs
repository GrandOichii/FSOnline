namespace FSManager.Shared.Models;

public class CardModel : CardTemplate {
    public required List<CardImage> Images { get; set; }
    public required CardCollection Collection { get; set; }

    public required List<CardModel> StartingItems { get; set; }
    public required List<CardModel> IsStartingItemFor { get; set; }
}