namespace FSManager.Dto.Cards;

// public class GetCard : CardTemplate {
public class GetCard {
    public required string Key { get; set; }
    public required string Name { get; set; }
    public required string Text { get; set; }
    public required string Collection { get; set; }
    public required string ImageUrl { get; set; }
}