namespace FSManager.Shared.Models;

public class CardModel : CardTemplate {
    public required string ImageUrl { get; set; }
    public required CardCollection Collection { get; set; }
}