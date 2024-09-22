namespace FSCore.Matches;

public class DamagePreventor {

    public virtual void Prevent(object from) {
        if (from is Player player) {
            player.Match.LogDebug("1 Damage was prevented from {LogName}", player.LogName);
            return;
        }

        // TODO monster
    }
}