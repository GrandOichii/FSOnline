namespace FSManager.Dto.Cards;

public class PostCard : CardTemplate {
    public required string CollectionKey { get; set; }
    public required string DefaultImageSrc { get; set; }
}