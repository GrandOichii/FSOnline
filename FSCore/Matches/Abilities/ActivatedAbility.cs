namespace FSCore.Matches.Abilities;

public class ActivatedAbility {
    /// <summary>
    /// Ability text
    /// </summary>
    public string Text { get; }
    /// <summary>
    /// Check function
    /// </summary>
    public LuaFunction CheckFunc { get; }
    /// <summary>
    /// Cost function
    /// </summary>
    public LuaFunction CostFunc { get; }
    /// <summary>
    /// Effects
    /// </summary>
    public EffectList Effects { get; }

    public ActivatedAbility(LuaTable table) {

        Text = LuaUtility.TableGet<string>(table, "Text");
        CheckFunc = LuaUtility.TableGet<LuaFunction>(table, "Check");
        CostFunc = LuaUtility.TableGet<LuaFunction>(table, "Cost");
        Effects = new(LuaUtility.TableGet<LuaTable>(table, "Effects"));
    }

    public bool CanBeActivatedBy(InPlayMatchCard card, Player player) {
        if (!card.CanBeActivatedBy(player)) return false;
        // TODO other checks

        try {
            var returned = CheckFunc.Call(card, player);
            return LuaUtility.GetReturnAsBool(returned);
        } catch (Exception e) {
            throw new MatchException($"Exception during check execution of activated ability of card {card.Card.LogName} by player {player.LogName}", e);
        }
    }

    public bool PayCosts(InPlayMatchCard card, Player player) {
        try {
            var returned = CostFunc.Call(card, player);
            return LuaUtility.GetReturnAsBool(returned);
        } catch (Exception e) {
            throw new MatchException($"Exception during cost execution of activated ability of card {card.Card.LogName} by player {player.LogName}", e);
        }
    }

    public void ExecuteEffects(StackEffect stackEffect) {
        try {
            Effects.Execute(stackEffect);
        } catch (Exception e) {
            throw new MatchException($"Failed to execute Effects of activated ability", e);
        }
    }
}