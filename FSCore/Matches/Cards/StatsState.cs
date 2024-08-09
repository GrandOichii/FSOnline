namespace FSCore.Matches.Cards;

public class StatsState {
    public int Attack { get; set; }
    public int Health { get; set; }
    public int? Evasion { get; set; } = null;

    public List<LuaFunction> ReceivedDamageModifiers { get; } = [];

    public StatsState() {
        Attack = -1;
        Health = -1;
    }

    public StatsState(Player player) {
        Attack = player.Character.GetTemplate().Attack;
        Health = player.Character.GetTemplate().Health;
    }

    public StatsState(InPlayMatchCard card) {
        if (card.Stats is null)
            throw new MatchException($"Tried to create state of stats for card {card.LogName}, which has no stats");

        Attack = card.Card.Template.Attack;
        Health = card.Card.Template.Health;
        Evasion = card.Card.Template.Evasion;
    }
}