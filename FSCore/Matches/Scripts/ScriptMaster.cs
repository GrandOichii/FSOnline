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

    [LuaCommand]
    public void AddLootPlay(int playerIdx, int amount) {
        var player = _match.GetPlayer(playerIdx);
        player.AddLootPlay(amount);
    }

    [LuaCommand]
    public void PayCoins(int playerIdx, int amount) {
        var player = _match.GetPlayer(playerIdx);
        player.PayCoins(amount);
    }

    [LuaCommand]
    public void CreateOwnedItem(MatchCard card, int ownerIdx) {
        var owner = _match.GetPlayer(ownerIdx);
        var result = new OwnedInPlayMatchCard(card, owner);
        
        _match.PlaceOwnedCard(result)
            .Wait();
    }

    [LuaCommand]
    public void Roll(StackEffect parentEffect) {
        _match.AddRoll(parentEffect)
            .Wait();
    }

    [LuaCommand]
    public void LoseCoins(int playerIdx, int amount) {
        _match.GetPlayer(playerIdx).LoseCoins(amount);
    }

    [LuaCommand]
    public void Recharge(string ipid) {
        var item = _match.GetInPlayCard(ipid);

        item.Untap()
            .Wait()
        ;
    }

    [LuaCommand]
    public void DestroyItem(string ipid) {
        _match.DestroyItem(ipid)
            .Wait();
    }

    [LuaCommand]
    public LuaTable GainTreasure(int playerIdx, int amount) {
        var result = _match
            .GetPlayer(playerIdx)
            .GainTreasure(amount)
        .GetAwaiter().GetResult();

        return LuaUtility.CreateTable(_match.LState, result);
    }

    [LuaCommand]
    public Player GetPlayer(int playerIdx) => _match.GetPlayer(playerIdx);

    [LuaCommand]
    public string PromptString(int playerIdx, LuaTable optionsTable, string hint) {
        var player = _match.GetPlayer(playerIdx);

        List<string> options = new();
        foreach (var v in optionsTable.Values)
            options.Add((string)v);

        var result = player.ChooseString(options, hint)
            .GetAwaiter().GetResult();

        return result;
    }

    [LuaCommand]
    public LuaTable GetPlayers() {
        return LuaUtility.CreateTable(_match.LState, _match.Players);
    }

    [LuaCommand]
    public int ChoosePlayer(int playerIdx, LuaTable optionsTable, string hint) {
        var options = LuaUtility.ParseTable(optionsTable);

        var player = _match.GetPlayer(playerIdx);
        var result = player.ChoosePlayer(options, hint)
            .GetAwaiter().GetResult();

        return result;
    }

    [LuaCommand]
    public void AddTarget(StackEffect effect, int type, string value) {
        effect.Targets.Add(new(
            (TargetType)type,
            value
        ));
    }
}