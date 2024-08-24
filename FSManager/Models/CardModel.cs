namespace FSManager.Models;

// public class CardModel1 : CardTemplate {

// }

public class CardModel {
    public required string Key { get; set; }
    public required string Name { get; set; }
    public required string Text { get; set; }

    public required List<CardImage> Images { get; set; }
    public required CardCollection Collection { get; set; }
}