namespace FSCore.Matches;

public class DamagePreventor {

    public virtual void Prevent(Player player) {
        player.Match.LogInfo($"1 Damage was prevented from {player.LogName}");
    }
}