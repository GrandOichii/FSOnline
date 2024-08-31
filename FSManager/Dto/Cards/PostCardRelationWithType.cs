namespace FSManager.Dto.Cards;

public class PostCardRelationWithType : PostCardRelation {
    public required CardRelationType RelationType { get; set; }
}