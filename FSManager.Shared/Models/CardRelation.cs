namespace FSManager.Shared.Models;

public enum CardRelationType {
    GENERAL = 0,
    STARTING_ITEM = 1,
    GUPPY_ITEM = 2

}

public class CardRelation {
    public int ID { get; set; }
    public required CardRelationType RelationType { get; set; }

    public required CardModel RelatedTo { get; set; }
    public required CardModel RelatedCard { get; set; }
}