
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

            Match.LogDebug(logMsg);

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
            Match.LogDebug("Resolving attack roll of {Roll} by player {PlayerLogName} to monster {CardLogName}", last, player.LogName, Monster.LogName);

            if (Monster.IsMiss(last)) {
                var effect = new DamageStackEffect(Match, OwnerIdx, Monster.GetAttack(), this, Match.GetPlayer(OwnerIdx))
                {
                    Roll = last
                };
                effect.SetParentEffect(this);

                await Match.PlaceOnStack(effect);

                // TODO trigger miss
            } else {
                // is not miss
                var effect = new DamageStackEffect(Match, OwnerIdx, player.GetAttack(), this, Monster)
                {
                    Roll = last
                };
                effect.SetParentEffect(this);

                await Match.PlaceOnStack(effect);

            }

            // TODO
            
            Rolls.Remove(last);
            return false;
        }

        await Match.AddRoll(this, true);

        return false;
    }

    public override StackEffectData ToData() => new AttackStackEffectData(this);
}
