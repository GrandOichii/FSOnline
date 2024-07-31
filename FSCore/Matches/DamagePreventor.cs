namespace FSCore.Matches;

public class DamagePreventor {

    public virtual void Prevent(object from) {
        if (from is Player player) {
            player.Match.LogInfo($"1 Damage was prevented from {player.LogName}");
            return;
        }

        // TODO monster
    }
}