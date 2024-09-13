namespace FSManager.Dto.Cards;

// TODO? should this be here


public class CardFilter {
    public string Name { get; set; } = "";
    public string Collection { get; set; } = "";
    public string Type { get; set; } = "";

    public IQueryable<CardModel> Modify(IQueryable<CardModel> query) {
        return query
            .Where(c => c.Name.ToLower().Contains(Name.ToLower()))
            .Where(c => string.IsNullOrEmpty(Collection) || c.Collection.Key == Collection)
            .Where(c => string.IsNullOrEmpty(Type) || c.Type == Type)
        ;
    }
}
