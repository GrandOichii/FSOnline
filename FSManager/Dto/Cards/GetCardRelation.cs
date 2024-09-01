namespace FSManager.Dto.Cards;

public class GetCardRelation {
    public required GetCard RelatedCard { get; set; }
    public required GetCard RelatedTo { get; set; }

    public required CardRelationType RelationType { get; set; }

    public GetCard GetRelatedCard(GetCard from) {
        if (RelatedCard.Key == from.Key) return RelatedTo;
        if (RelatedTo.Key == from.Key) return RelatedCard;

        throw new ArgumentException($"Card {from.Key} doesn't participate in relation {RelatedTo.Key} -> {RelatedCard.Key}");
    }
}