namespace FSCore.Matches.Cards;

public struct StatsState {
    public int Attack { get; set; }
    public int Health { get; set; }
    public int? Evasion { get; set; }    

    public StatsState(Player player) {
        Attack = player.Character.GetTemplate().Attack;
        Health = player.Character.GetTemplate().Health;
    }
}