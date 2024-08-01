namespace FSCore.Matches.Cards;

/// <summary>
/// Game object stats
/// </summary>
public class Stats {
    public StatsState State { get; set; } = new();
    public int Damage { get; set; }
    public List<DamagePreventor> DamagePreventors { get; } = [];

    public void UpdateState(Player player) {
        State = new(player);
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

    public async Task ProcessDamage(Player player, int amount, StackEffect source) {

        amount = PreventDamage(amount);
        if (amount == 0) return;

        player.Match.LogInfo($"Player {player.LogName} was dealt {amount} damage");
        Damage += amount;
        if (Damage >= State.Health) {
            Damage = State.Health;
            player.DeathSource = source;
        }

        await player.Match.Emit("player_damaged", new() {
            { "Player", this },
            { "Amount", amount },
            { "Source", source },
        });
    }


    // public bool CanBeAttacked() {
    //     if (Evasion is null) return false;
    //     // TODO more

    //     return true;
    // }
}