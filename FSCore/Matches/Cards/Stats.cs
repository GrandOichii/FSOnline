namespace FSCore.Matches.Cards;

/// <summary>
/// Game object stats
/// </summary>
public class Stats {
    public StatsState State { get; set; } = new();
    public int Damage { get; set; }
    public List<DamagePreventor> DamagePreventors { get; } = [];
    public StackEffect? DeathSource { get; set; } = null;
    public bool IsDead { get; set; } = false;

    public void UpdateState(Player player) {
        State = new(player);
    }

    public void UpdateState(InPlayMatchCard card) {
        State = new(card);
    }

    public void AddDamagePreventors(int amount) {
        for (int i = 0; i < amount; i++) {
            var p = new DamagePreventor();
            DamagePreventors.Add(p);
        }
        // TODO? update
    }

    public int PreventDamage(int amount) {
        while (amount > 0) {
            if (DamagePreventors.Count == 0) break;
            var preventor = DamagePreventors[0];
            preventor.Prevent(this);
            DamagePreventors.Remove(preventor);
            --amount;
        }

        return amount;
    }

    public async Task ProcessDamage(InPlayMatchCard card, int amount, DamageStackEffect effect) {
        amount = ModifyDealtDamage(amount, effect);
        if (amount == 0) return;

        var original = amount;
        amount = PreventDamage(amount);
        card.Card.Match.LogDebug("Prevented {Prevented} damage out of {Damage} for card {CardLogName}", original-amount, original, card.LogName);
        if (amount == 0) return;

        var match = card.Card.Match;

        match.LogDebug("Card {LogName} was dealt {Amount} damage", card.LogName, amount);
        Damage += amount;
        CheckDead(effect);

        await match.Emit("card_damaged", new() {
            { "Card", card },
            { "Amount", amount },
            { "Effect", effect },
        });
    }


    private int ModifyDealtDamage(int amount, DamageStackEffect stackEffect) {
        // TODO catch exceptions
        foreach (var mod in State.ReceivedDamageModifiers) {
            var returned = mod.Call(amount, stackEffect);
            amount = LuaUtility.GetReturnAsInt(returned);
        }
        
        return amount;
    }
    
    public async Task ProcessDamage(Player player, int amount, DamageStackEffect stackEffect) {
        amount = ModifyDealtDamage(amount, stackEffect);
        if (amount == 0) return;
        
        var original = amount;
        amount = PreventDamage(amount);
        player.Match.LogDebug("Prevented {Prevented} damage out of {Damage} for player {PlayerLogName}", original-amount, original, player.LogName);
        if (amount == 0) return;

        player.Match.LogDebug("Player {LogName} was dealt {Amount} damage", player.LogName, amount);
        Damage += amount;
        CheckDead(stackEffect);

        await player.Match.Emit("player_damaged", new() {
            { "Player", player },
            { "Amount", amount },
            { "Effect", stackEffect },
        });
    }

    public void CheckDead(StackEffect possibleSource) {
        if (Damage < State.Health) return;

        Damage = State.Health;
        DeathSource = possibleSource;
    }


    // public bool CanBeAttacked() {
    //     if (Evasion is null) return false;
    //     // TODO more

    //     return true;
    // }

    public int GetCurrentHealth() => State.Health - Damage;

    public int GetEvasion() {
        var e = State.Evasion
            ?? throw new MatchException($"Tried to get evasion from game object with no evasion")
        ;

        return Math.Clamp(e, 1, 6);
    }

}