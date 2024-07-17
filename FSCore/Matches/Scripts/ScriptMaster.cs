using System.Linq.Expressions;
using System.Reflection;

namespace FSCore.Matches.Scripts;

/// <summary>
/// Creates a number of useful lua commands on initialization (can be discarded afterwards)
/// </summary>
public class ScriptMaster {
    /// <summary>
    /// Match
    /// </summary>
    private readonly Match _match;
    public ScriptMaster(Match match) {
        _match = match;

        // load all methods into the Lua state
        var type = typeof(ScriptMaster);
        foreach (var method in type.GetMethods())
        {
            if (method.GetCustomAttribute(typeof(LuaCommand)) is not null)
            {
                _match.LState[method.Name] = method.CreateDelegate(Expression.GetDelegateType(
                    (from parameter in method.GetParameters() select parameter.ParameterType)
                    .Concat(new[] { method.ReturnType })
                .ToArray()), this);
            }
        }
    }

    [LuaCommand]
    public void AddCoins(int playerIdx, int amount) {
        var player = _match.GetPlayer(playerIdx);
        player.GainCoins(amount)
            .Wait();
    }

    [LuaCommand]
    public void LootCards(int playerIdx, int amount, StackEffect parentEffect) {
        var player = _match.GetPlayer(playerIdx);

        player.LootCards(
            amount,
            LootReasons.Effect(_match.LState, parentEffect)
        ).Wait();
    }

    [LuaCommand]
    public void TapCard(string ipid) {
        var item = _match.GetInPlayCard(ipid);

        item.Tap()
            .Wait();
    }

}