namespace FSManager.Dto.Cards;

public class GetCardRelation {
    public required GetCard Card { get; set; }
    public required CardRelationType RelationType { get; set; }
}