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
    public bool DestroyItem(string ipid) {
        return _match.DestroyItem(ipid)
            .GetAwaiter().GetResult();
    }

    [LuaCommand]
    public void DiscardFromPlay(string ipid) {
        _match.DiscardFromPlay(ipid)
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

        List<string> options = [];
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
    public LuaTable GetPlayersInTurnOrder(int firstIdx) {
        var result = new List<int>();

        var cur = firstIdx;
        while (true) {
            result.Add(cur);
            cur = _match.NextInTurnOrder(cur);
            if (cur == result[0]) break;
        }

        return LuaUtility.CreateTable(_match.LState, result.Select(idx => _match.GetPlayer(idx)).ToList());
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
    public string ChooseStackEffect(int playerIdx, LuaTable optionsTable, string hint) {
        var options = LuaUtility.ParseTable<string>(optionsTable);

        var player = _match.GetPlayer(playerIdx);
        var result = player.ChooseStackEffect(options, hint)
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

    [LuaCommand]
    public void RerollDice(StackEffect stackEffect) {
        if (stackEffect is not RollStackEffect rollEffect)
            throw new MatchException($"Tried to reroll a non-roll stack effect: {stackEffect}");

        rollEffect.Reroll();
    }

    [LuaCommand]
    public void SetRollValue(StackEffect stackEffect, int value) {
        if (stackEffect is not RollStackEffect rollEffect)
            throw new MatchException($"Tried to reroll a non-roll stack effect: {stackEffect}");

        rollEffect.SetValue(value);
    }

    [LuaCommand]
    public void ModRoll(StackEffect stackEffect, int mod) {
        if (stackEffect is not RollStackEffect rollEffect)
            throw new MatchException($"Tried to modify a non-roll stack effect: {stackEffect}");

        rollEffect.Modify(mod);
    }

    [LuaCommand]
    public LuaTable GetStackEffects() {
        return LuaUtility.CreateTable(_match.LState, _match.Stack.Effects);
    }

    [LuaCommand]
    public bool IsRollStackEffect(StackEffect stackEffect) {
        return stackEffect is RollStackEffect;
    }

    [LuaCommand]
    public bool IsAbilityActivation(StackEffect stackEffect) {
        return stackEffect is ActivatedAbilityStackEffect;
    }

    [LuaCommand]
    public bool IsLootStackEffect(StackEffect stackEffect) {
        return stackEffect is LootCardStackEffect;
    }

    [LuaCommand]
    public bool IsTrigger(StackEffect stackEffect) {
        return stackEffect is TriggeredAbilityStackEffect;
    }

    [LuaCommand]
    public StackEffect GetStackEffect(string sid) {
        return _match.Stack.Effects.First(se => se.SID == sid);
    }

    // TODO only allows to pick a card in your own hand
    [LuaCommand]
    public int ChooseCardInHand(int playerIdx, LuaTable optionsTable, string hint) {
        var options = LuaUtility.ParseTable(optionsTable);

        var player = _match.GetPlayer(playerIdx);
        var result = player.ChooseCardInHand(options, hint)
            .GetAwaiter().GetResult();

        return result;
    }

    [LuaCommand]
    public string ChooseItem(int playerIdx, LuaTable optionsTable, string hint) {
        var options = LuaUtility.ParseTable<string>(optionsTable);

        var player = _match.GetPlayer(playerIdx);
        var result = player.ChooseItem(options, hint)
            .GetAwaiter().GetResult();

        return result;
   
    }

    [LuaCommand]
    public void DiscardFromHand(int playerIdx, int handIdx) {
        var player = _match.GetPlayer(playerIdx);
        player.DiscardFromHand(handIdx)
            .Wait();
    }

    [LuaCommand]
    public void SoftReloadState() {
        _match.SoftReloadState()
            .Wait();
    }

    [LuaCommand]
    public void PutGenericCounters(string ipid, int amount) {
        var item = _match.GetInPlayCard(ipid);

        item.AddCounters(amount)
            .Wait();
    }

    [LuaCommand]
    public int GetCountersCount(string ipid) {
        var item = _match.GetInPlayCard(ipid);
        return item.GetCountersCount();
    }

    [LuaCommand]
    public void RemoveCounters(string ipid, int amount) {
        var item = _match.GetInPlayCard(ipid);
        item.RemoveCounters(amount)
            .Wait();
    }

    [LuaCommand]
    public LuaTable GetItems() {
        return LuaUtility.CreateTable(_match.LState, _match.GetItems());
    }

    [LuaCommand]
    public void RerollItem(string ipid) {
        _match.RerollItem(ipid)
            .Wait();
    }

    [LuaCommand]
    public bool IsOwned(InPlayMatchCard card) {
        return card is OwnedInPlayMatchCard;
    }

    [LuaCommand]
    public void StealItem(int playerIdx, string ipid) {
        _match.StealItem(playerIdx, ipid)
            .Wait();
    }

    [LuaCommand]
    public InPlayMatchCard? GetItemOrDefault(string ipid) {
        return _match.GetItemOrDefault(ipid);
    }

    [LuaCommand]
    public int GetCurPlayerIdx() {
        return _match.CurPlayerIdx;
    }

    [LuaCommand]
    public LuaTable GetShopSlots() {
        return LuaUtility.CreateTable(_match.LState, _match.TreasureSlots);
    }

    [LuaCommand]
    public bool IsShopItem(InPlayMatchCard card) {
        return _match.TreasureSlots.FirstOrDefault(slot => slot.Card == card) is not null;
    }

    [LuaCommand]
    public void AddSoulCard(int playerIdx, MatchCard card) {
        _match.AddSoulCard(playerIdx, card)
            .Wait();
    }

    [LuaCommand]
    public void RemoveFromPlay(string ipid) {
        _match.RemoveFromPlay(_match.GetInPlayCard(ipid))
            .Wait();
    }

    [LuaCommand]
    public bool IsPresent(string ipid) {
        return _match.GetInPlayCardOrDefault(ipid) is not null;
    }

    [LuaCommand]
    public bool RemoveFromBonusSouls(string id) {
        var removed = _match.BonusSouls.Remove(
            _match.BonusSouls.First(card => card.ID == id)
        );

        return removed;
    }

    [LuaCommand]
    public void ExpandShotSlots(int amount) {
        _match.ExpandShotSlots(amount)
            .Wait();
    }

    [LuaCommand]
    public void StealCoins(int playerIdx, int fromIdx, int amount) {
        var p1 = _match.GetPlayer(playerIdx);
        var p2 = _match.GetPlayer(fromIdx);
        p2.LoseCoins(amount);
        p1.GainCoinsRaw(amount);
    }

    [LuaCommand]
    public void RemoveFromHand(int playerIdx, int handIdx) {
        var player = _match.GetPlayer(playerIdx);
        player.Hand.RemoveAt(handIdx);
    }

    [LuaCommand]
    public void AddToHand(int playerIdx, MatchCard card) {
        _match.GetPlayer(playerIdx).AddToHand(card)
            .Wait();
    }

    [LuaCommand]
    public InPlayMatchCard GetInPlayCard(string ipid){
        return _match.GetInPlayCard(ipid);
    }

    [LuaCommand]
    public void DealDamageToPlayer(int toIdx, int amount, StackEffect sourceEffect) {
        if (sourceEffect.OwnerIdx == -1) {
            _match.DamageToPlayerRequest(toIdx, amount, sourceEffect)
                .Wait();
            return;
        }
        var owner = _match.GetPlayer(sourceEffect.OwnerIdx);
        owner.DamageToPlayerRequest(toIdx, amount, sourceEffect)
            .Wait();
    }

    [LuaCommand]
    public void DealDamageToCard(string ipid, int amount, StackEffect sourceEffect) {
        _match.DamageToCardRequest(ipid, amount, sourceEffect)
            .Wait();
    }

    [LuaCommand]
    public void KillPlayer(int playerIdx, StackEffect source) {
        var player = _match.GetPlayer(playerIdx);
        player.PushDeath(source)
            .Wait();
    }

    [LuaCommand]
    public void RemoveFromEverywhere(string id) {
        foreach (var player in _match.Players) {
            var removed = player.Remove(id);
            if (removed) return;
        }
        foreach (var deck in _match.DeckIndex.Values) {
            var removed = deck.Remove(id);
            if (removed) return;
        }

        var card = _match.BonusSouls.FirstOrDefault(c => c.ID == id);
        if (card is not null) {
            _match.BonusSouls.Remove(card);
            return;
        }

        throw new MatchException($"Failed to find card with ID {id} to remove");
    }

    [LuaCommand]
    public int GetSoulCount(int playerIdx) {
        return _match.GetPlayer(playerIdx).SoulCount();
    }

    [LuaCommand]
    public void CancelEffect(string sid) {
        _match.CancelEffect(sid);
    }

    [LuaCommand]
    public void TillEndOfTurn(int layer, LuaFunction effect) {
        var key = (ModificationLayer)layer;
        if (!_match.TEOTEffects.TryGetValue(key, out List<LuaFunction>? list)) {
            list = [];
            _match.TEOTEffects.Add(key, list);
        }

        list.Add(effect);
    }

    [LuaCommand]
    public void AddGenericDamagePreventors(int playerIdx, int amount) {
        _match.GetPlayer(playerIdx).AddDamagePreventors(amount)
            .Wait();
    }

    [LuaCommand]
    public void LoseHealth(int playerIdx, int amount, StackEffect source) {
        _match.GetPlayer(playerIdx).LoseHealth(amount, source)
            .Wait();
    }

    [LuaCommand]
    public string ChooseInPlayCard(int playerIdx, LuaTable optionsTable, string hint) {
        // TODO? should be different
        return ChooseItem(playerIdx, optionsTable, hint);
    }

    [LuaCommand]
    public void EndTheTurn() {
        _match.TurnEnded = true;
    }

    [LuaCommand]
    public void AddDeathPreventor(int playerIdx, LuaFunction preventorFunc) {
        var player = _match.GetPlayer(playerIdx);
        player.AddDeathPreventor(preventorFunc);
    }

    [LuaCommand]
    public void CancelEverything() {
        // TODO? more
        _match.Stack.Effects.Clear();
    }

    [LuaCommand]
    public LuaTable GetMonsters() {
        return LuaUtility.CreateTable(_match.LState, _match.GetMonsters());
    }
}