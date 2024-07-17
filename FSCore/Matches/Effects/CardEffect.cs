namespace FSCore.Matches.Effects;

/// <summary>
/// Effect of a card
/// </summary>
public class CardEffect : Effect {
    /// <summary>
    /// Parent card
    /// </summary>
    public MatchCard Card { get; }

    public CardEffect(MatchCard card, LuaTable data)
        :base(data) 
    {
        Card = card;

    }

    /// <summary>
    /// Execute the effect
    /// </summary>
    /// <param name="stackEffect">Parent stack effect</param>
    public override void Execute(StackEffect stackEffect) {
        try {
            base.Execute(stackEffect);
        } catch (Exception e) {
            throw new MatchException($"Failed to execute CardEffect of card {Card.LogName}", e);
        }
    }
}