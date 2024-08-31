namespace FSManager.Dto.Cards;

public class GetCardWithRelations : GetCard {
    public required List<GetCard> RelatedCards { get; set; }
}