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

    public async Task ProcessDamage(InPlayMatchCard card, int amount, StackEffect source) {

        amount = PreventDamage(amount);
        if (amount == 0) return;

        var match = card.Card.Match;

        match.LogInfo($"Card {card.LogName} was dealt {amount} damage");
        Damage += amount;
        CheckDead(source);

        await match.Emit("card_damaged", new() {
            { "Card", card },
            { "Amount", amount },
            { "Source", source },
        });
    }
    public async Task ProcessDamage(Player player, int amount, StackEffect source) {

        amount = PreventDamage(amount);
        if (amount == 0) return;

        player.Match.LogInfo($"Player {player.LogName} was dealt {amount} damage");
        Damage += amount;
        CheckDead(source);

        await player.Match.Emit("player_damaged", new() {
            { "Player", this },
            { "Amount", amount },
            { "Source", source },
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
}