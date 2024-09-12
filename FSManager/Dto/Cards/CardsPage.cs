namespace FSManager.Dto.Cards;

public class CardsPage {
    public required List<GetCard> Cards { get; set; }
    public required int Page { get; set; }
    public required int PageCount { get; set; }
}
