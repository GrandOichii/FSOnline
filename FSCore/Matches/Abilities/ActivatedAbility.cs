namespace FSCore.Matches.Abilities;

public class ActivatedAbility {
    /// <summary>
    /// Ability cost text
    /// </summary>
    public string CostText { get; }
    /// <summary>
    /// Ability effect text
    /// </summary>
    public string EffectText { get; }
    /// <summary>
    /// Check function
    /// </summary>
    public LuaFunction CheckFunc { get; }
    /// <summary>
    /// Cost function
    /// </summary>
    public LuaFunction CostFunc { get; }
    /// <summary>
    /// Check function for if the effect should fizzle
    /// </summary>
    public LuaFunction FizzleCheck { get; }
    /// <summary>
    /// Effects
    /// </summary>
    public EffectList Effects { get; }

    public ActivatedAbility(LuaTable table) {

        CostText = LuaUtility.TableGet<string>(table, "CostText");
        EffectText = LuaUtility.TableGet<string>(table, "EffectText");
        CheckFunc = LuaUtility.TableGet<LuaFunction>(table, "Check");
        CostFunc = LuaUtility.TableGet<LuaFunction>(table, "Cost");
        Effects = new(LuaUtility.TableGet<LuaTable>(table, "Effects"));
        FizzleCheck = LuaUtility.TableGet<LuaFunction>(table, "FizzleCheck");
    }

    public bool ShouldFizzle(StackEffect effect) {
        try {
            var returned = FizzleCheck.Call(effect);
            return !LuaUtility.GetReturnAsBool(returned);
        } catch (Exception e) {
            throw new MatchException($"Exception during fizzle check execution of activated ability {effect.SID}", e);
        }

    }

    public bool CanBeActivatedBy(InPlayMatchCard card, Player player) {
        if (!card.CanBeActivatedBy(player)) return false;
        // TODO other checks

        try {
            var returned = CheckFunc.Call(card, player);
            return LuaUtility.GetReturnAsBool(returned);
        } catch (Exception e) {
            throw new MatchException($"Exception during check execution of activated ability of card {card.LogName} by player {player.LogName}", e);
        }
    }

    public bool PayCosts(InPlayMatchCard card, Player player, StackEffect stackEffect) {
        try {
            var returned = CostFunc.Call(card, player, stackEffect);
            return LuaUtility.GetReturnAsBool(returned);
        } catch (Exception e) {
            throw new MatchException($"Exception during cost execution of activated ability of card {card.LogName} by player {player.LogName}", e);
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