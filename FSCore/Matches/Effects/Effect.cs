namespace FSCore.Matches.Effects;

/// <summary>
/// Effect
/// </summary>
public class Effect {
    /// <summary>
    /// Effect function
    /// </summary>
    public LuaFunction EffectFunc { get; }

    public Effect(LuaFunction func) {
        // EffectFunc = LuaUtility.TableGet<LuaFunction>(data, "effect");
        EffectFunc = func;
    }

    /// <summary>
    /// Execute the effect
    /// </summary>
    /// <param name="stackEffect">Parent stack effect</param>
    public virtual void Execute(StackEffect stackEffect) {
        EffectFunc.Call(stackEffect);
    }
}