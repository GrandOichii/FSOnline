
namespace FSCore.Matches.Cards;

public class BonusSoulMatchCard : MatchCard, IStateModifier
{
    public BonusSoulMatchCard(Match match, CardTemplate template)
        : base(match, template, null)
    {
        
    }

    public void Modify(ModificationLayer layer)
    {
        // TODO move to a getter function
        if (!StateModifiers.TryGetValue(layer, out List<LuaFunction>? stateModifiers)) return;

        try {
            foreach (var mod in stateModifiers) {
                mod.Call(this);
            }
        } catch (Exception e) {
            throw new MatchException($"Failed to execute state modification function of bonus soul card {LogName}", e);
        }
    }

    public void UpdateState()
    {
        // TODO
    }
}