namespace FSCore.Matches.Effects;

public class EffectList {
    /// <summary>
    /// Effects
    /// </summary>
    public List<Effect> Effects { get; }

    public EffectList(LuaTable table) {
        Effects = new();
        foreach (var o in table.Values) {
            var func = o as LuaFunction
                ?? throw new MatchException($"Expected effect to be a table, but found {o.GetType()}")
            ;
            Effects.Add(new(func));
        }
    }

    /// <summary>
    /// Execute all effects
    /// </summary>
    /// <param name="stackEffect">Parent stack effect</param>
    public void Execute(StackEffect stackEffect) {
        foreach (var effect in Effects)
            effect.Execute(stackEffect);
    }
}