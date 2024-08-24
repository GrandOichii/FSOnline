namespace FSManager.Models;

public class CardCollection {
    public required string Key { get; set; }

    public required List<CardModel> Cards { get; set; }
}