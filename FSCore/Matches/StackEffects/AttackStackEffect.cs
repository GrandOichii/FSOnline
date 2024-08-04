
namespace FSCore.Matches.StackEffects;

public class AttackStackEffect : StackEffect
{
    public InPlayMatchCard Monster { get; }
    public List<int> Participants { get; } // TODO utilize
    
    public AttackStackEffect(Match match, int ownerIdx, InPlayMatchCard monster)
        : base(match, ownerIdx)
    {
        Participants = [ ownerIdx ];
        Monster = monster;

        Targets.Add(new(TargetType.ITEM, monster.IPID));
    }


    public override async Task<bool> Resolve()
    {
        var player = Match.GetPlayer(OwnerIdx);
        var monsterDead = Monster.Stats!.IsDead;
        var playerDead = player.Stats.IsDead;
        if (monsterDead || playerDead) {
            var logMsg = "Attack ends: ";

            if (monsterDead)
                logMsg += $"|monster {Monster.LogName} died|";
            if (playerDead)
                logMsg += $"|player {player.LogName} died|";

            Match.LogInfo(logMsg);

            // ! When an attack ends due to a player or monster dying (or an attack being canceled), any unresolved attack rolls and combat damage are removed from the stack.
            // var newEffects = new List<StackEffect>();
            // foreach (var effect in Match.Stack.Effects) {
            //     if (effect.Parent != this)
            //         newEffects.Add(effect);
            // }
            // Match.Stack.Effects.
            return true;
        }

        if (Rolls.Count > 0) {
            var last = Rolls.Last();
            Match.LogInfo($"Resolving attack roll of {last} by player {player.LogName} to monster {Monster.LogName}");

            if (Monster.IsMiss(last)) {
                // TODO specify that is combat damage
                var effect = new DamageStackEffect(Match, OwnerIdx, Monster.GetAttack(), this, Match.GetPlayer(OwnerIdx));
                effect.SetParentEffect(this);

                await Match.PlaceOnStack(effect);

                // TODO trigger miss
            } else {
                // is not miss

                var effect = new DamageStackEffect(Match, OwnerIdx, player.GetAttack(), this, Monster);
                effect.SetParentEffect(this);

                await Match.PlaceOnStack(effect);

            }

            // TODO
            
            Rolls.Remove(last);
            return false;
        }

        // TODO specify that is combat roll
        await Match.AddRoll(this);

        return false;
    }

    public override StackEffectData ToData() => new AttackStackEffectData(this);
}
