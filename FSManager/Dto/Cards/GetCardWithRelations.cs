namespace FSManager.Dto.Cards;

public class GetCardWithRelations : GetCard {
    public required List<GetCardRelation> Relations { get; set; }
}