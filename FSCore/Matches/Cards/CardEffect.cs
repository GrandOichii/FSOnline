namespace FSCore.Matches.Effects;

/// <summary>
/// Effect of a card
/// </summary>
public class CardEffect {
    /// <summary>
    /// Parent card
    /// </summary>
    public MatchCard Card { get; }
    /// <summary>
    /// Effect function
    /// </summary>
    public LuaFunction EffectFunc { get; }

    public CardEffect(MatchCard card, LuaTable data) {
        Card = card;

        EffectFunc = LuaUtility.TableGet<LuaFunction>(data, "effect");
    }

    /// <summary>
    /// Execute the effect
    /// </summary>
    /// <param name="stackEffect">Parent stack effect</param>
    public void ExecuteEffect(StackEffect stackEffect) {
        try {
            EffectFunc.Call(stackEffect);
        } catch (Exception e) {
            throw new MatchException($"Failed to execute CardEffect of card {Card.LogName}", e);
        }
    }
}