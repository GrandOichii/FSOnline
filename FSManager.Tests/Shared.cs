namespace FSManager.Tests;

class Relation {
    public required CardModel Related { get; set; }
    public required CardRelationType Type { get; set; }
}

public class CardModelBuilder(string key) {
    private readonly string _collection = "col1";
    private readonly string _key = key;
    private readonly string _imageUrl = "http://card.image";
    private readonly string _name = "Card Name";
    private readonly string _rewardsText = "rewards text here";
    private readonly string _script = "print('no script provided')";
    private readonly int _soulValue = 0;
    private readonly string _text = "(no card text)";
    private readonly string _type = "Loot";
    private readonly List<Relation> _relations = [];
    private readonly List<Relation> _relatedTo = [];

    public CardModel Build() {
        var result = new CardModel() {
            Collection = new CardCollection { Key = _collection, Cards = []},
            ImageUrl = _imageUrl,
            Key = _key,
            Name = _name,
            RewardsText = _rewardsText,
            Script = _script,
            SoulValue = _soulValue,
            Text = _text,
            Type = _type,
            Relations = [],
            RelatedTo = []
        };

        foreach (var r in _relations) {
            var relation = new CardRelation {
                RelationType = r.Type,
                RelatedCard = r.Related,
                RelatedTo = result
            };
            result.Relations.Add(relation);
            r.Related.RelatedTo.Add(relation);
        }

        foreach (var r in _relatedTo) {
            var relation = new CardRelation {
                RelationType = r.Type,
                RelatedTo = r.Related,
                RelatedCard = result
            };
            result.RelatedTo.Add(relation);
            r.Related.Relations.Add(relation);
        }

        return result;
    }

    public CardModelBuilder HasRelatedCard(CardModel card, CardRelationType type) {
        _relations.Add(new() {
            Related = card,
            Type = type
        });
        
        return this;
    }

    public CardModelBuilder IsRelatedTo(CardModel card, CardRelationType type) {
        _relatedTo.Add(new() {
            Related = card,
            Type = type
        });
        
        return this;
    }
}