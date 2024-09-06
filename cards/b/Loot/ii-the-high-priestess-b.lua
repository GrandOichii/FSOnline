-- status: not tested

function _Create()
    return FS.B.Loot('Choose a player or monster, then roll: deal damage to that target equal to the number rolled.')
        .Target:MonsterOrPlayer()
        .Effect:Roll(
            FS.C.Effect.SwitchRoll(0, {
                [1] = FS.C.Effect.DamageToTarget(0, 1),
                [2] = FS.C.Effect.DamageToTarget(0, 2),
                [3] = FS.C.Effect.DamageToTarget(0, 3),
                [4] = FS.C.Effect.DamageToTarget(0, 4),
                [5] = FS.C.Effect.DamageToTarget(0, 5),
                [6] = FS.C.Effect.DamageToTarget(0, 6),
            })
        )
    :Build()
end