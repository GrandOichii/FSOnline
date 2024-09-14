using System.ComponentModel.DataAnnotations;

namespace FSManager.Dto.Cards;

public class PostCard : CardTemplate, IValidatableObject {
    public required string CollectionKey { get; set; }
    public required string ImageUrl { get; set; }


    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(Key)) yield return new("Card key can't be null or empty");
        if (string.IsNullOrEmpty(Name)) yield return new("Card name can't be null or empty");
        if (string.IsNullOrEmpty(CollectionKey)) yield return new("Card collection key can't be null or empty");

        if (string.IsNullOrEmpty(Type)) yield return new("Card type key can't be null or empty");
        // TODO could be better
        if (
            !new List<string>() {"Item", "StartingItem", "Monster", "Loot", "Curse", "Event", "BonusSoul", "Character" }
                .Contains(Type)
        ) yield return new($"Unrecognized card type: {Type}");

        if (SoulValue < 0) yield return new($"Card soul value can't be less than zero");
        if (!Uri.IsWellFormedUriString(ImageUrl, UriKind.Absolute)) yield return new($"Card image is not a well formed URI string: {ImageUrl}");

        // TODO validate script
    }
}