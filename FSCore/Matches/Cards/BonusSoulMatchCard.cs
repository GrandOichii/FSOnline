
namespace FSCore.Matches.Cards;

public class BonusSoulMatchCard : MatchCard, IStateModifier
{
    public BonusSoulMatchCard(Match match, CardTemplate template)
        : base(match, template, null)
    {
        
    }

    public void Modify(ModificationLayer layer)
    {
        if (!StateModifiers.ContainsKey(layer)) return;

        var stateModifiers = StateModifiers[layer];
        foreach (var mod in stateModifiers) {
            mod.Call(this);
        }
    }

    public void UpdateState()
    {
        // TODO
    }
}