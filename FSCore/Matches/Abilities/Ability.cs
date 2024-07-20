namespace FSCore.Matches.Abilities;

public abstract class Ability {
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

    public Ability(LuaTable table) {
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
            throw new MatchException($"Exception during fizzle check execution of ability {effect.SID}", e);
        }
    }

    public bool PayCosts(InPlayMatchCard card, Player? player, StackEffect stackEffect, LuaTable? args = null) {
        try {
            var returned = CostFunc.Call(card, player, stackEffect, args);
            return LuaUtility.GetReturnAsBool(returned);
        } catch (Exception e) {
            throw new MatchException($"Exception during cost execution of ability of card {card.LogName} by player {player.LogName}", e);
        }
    }

    public bool ExecuteCheck(InPlayMatchCard card, Player? player, LuaTable? args = null) {
        try {
            var returned = CheckFunc.Call(card, player, args);
            return LuaUtility.GetReturnAsBool(returned);
        } catch (Exception e) {
            throw new MatchException($"Exception during check execution of activated ability of card {card.LogName} by player {player.LogName}", e);
        }
    }

    public void ExecuteEffects(StackEffect stackEffect, LuaTable? args = null) {
        try {
            Effects.Execute(stackEffect, args);
        } catch (Exception e) {
            throw new MatchException($"Failed to execute Effects of activated ability", e);
        }
    }

}