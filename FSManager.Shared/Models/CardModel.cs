namespace FSManager.Shared.Models;

public class CardModel : CardTemplate {
    public required string ImageUrl { get; set; }

    public required CardCollection Collection { get; set; }
    public required List<CardRelation> Relations { get; set; }
    public required List<CardRelation> RelatedTo { get; set; }

    public CardRelation? GetRelationWith(string key) {
        var result = Relations.FirstOrDefault(r => r.RelatedCard.Key == key);
        if (result is not null) return result;

    return RelatedTo.FirstOrDefault(r => r.RelatedTo.Key == key);
    }
}