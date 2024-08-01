namespace FSCore.Matches.Cards;

public class StatsState {
    public int Attack { get; set; }
    public int Health { get; set; }
    public int? Evasion { get; set; }    

    public StatsState() {
        Attack = -1;
        Health = -1;
    }

    public StatsState(Player player) {
        Attack = player.Character.GetTemplate().Attack;
        Health = player.Character.GetTemplate().Health;
    }
}