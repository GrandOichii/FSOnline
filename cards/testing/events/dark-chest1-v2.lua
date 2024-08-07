-- status: not tested

function _Create()
    return FS.B.Event()
        .Effect:Roll(
            FS.C.Effect.SwitchRoll(0, {
                [1] = FS.C.Effect.Loot(1),
                [2] = FS.C.Effect.Loot(1),
                [3] = FS.C.Effect.GainCoins(3),
                [4] = FS.C.Effect.GainCoins(3),
                [5] = FS.C.Effect.DamageToPlayer(2),
                [6] = FS.C.Effect.DamageToPlayer(2),
            })
        )
        -- Text here
        
    :Build()
end