namespace FSManager.Shared.Models;

public class CardModel : CardTemplate {
    public required string ImageUrl { get; set; }

    public required CardCollection Collection { get; set; }
    public required List<CardRelation> Relations { get; set; }
    public required List<CardRelation> RelatedTo { get; set; }
}