namespace FSManager.Dto.Cards;

// TODO? should this be here


public class CardFilter {
    public string Name { get; set; } = "";
    public string Collection { get; set; } = "";
    public string Type { get; set; } = "";

    public string OrderBy { get; set; } = "";

    public IQueryable<CardModel> Modify(IQueryable<CardModel> query) {
        var result = query
            .Where(c => c.Name.ToLower().Contains(Name.ToLower()))
            .Where(c => string.IsNullOrEmpty(Collection) || c.Collection.Key == Collection)
            .Where(c => string.IsNullOrEmpty(Type) || c.Type == Type)
        ;
        return OrderBy switch
        {
            "Type" => result.OrderBy(c => c.Type),
            "Name" => result.OrderBy(c => c.Name),
            "Collection" => result.OrderBy(c => c.Collection),
            _ => result.OrderBy(c => c.Key),
        };
    }
}
