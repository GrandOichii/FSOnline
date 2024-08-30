namespace FSManager.Shared.Models;

public class CardModel : CardTemplate {
    public required List<CardImage> Images { get; set; }
    public required CardCollection Collection { get; set; }
}