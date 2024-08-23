namespace FSManager.Dto.Cards;

// public class CreateCard : CardTemplate {
public class CreateCard {
    public required string Key { get; set; }
    public required string Name { get; set; }
    public required string Text { get; set; }
    public required string DefaultImageSrc { get; set; }
}