namespace FSManager.Dto.Cards;

public class GetCard : CardTemplate {
    public required string Collection { get; set; }
    public required string ImageUrl { get; set; }
}