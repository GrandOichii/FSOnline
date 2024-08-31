namespace FSManager.Shared.Models;

public enum CardRelationType {
    STARTING_ITEM = 0
}

public class CardRelation {
    public required int ID { get; set; }
    public required CardRelationType RelationType { get; set; }

    public required CardModel RelatedTo { get; set; }
    public required CardModel RelatedCard { get; set; }
}