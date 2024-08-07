-- status: not tested

function _Create()
    return FS.B.Event()
        .Effect:Roll(
            FS.C.Effect.SwitchRoll(0, {
                [1] = FS.C.Effect.DamageToPlayer(3),
                [2] = FS.C.Effect.Discard(2),
                [3] = FS.C.Effect.Discard(2),
                [4] = FS.C.Effect.GainCoins(7),
                [5] = FS.C.Effect.GainCoins(7),
                [6] = FS.C.Effect.GainTreasure(1),
            })
        )
    :Build()
end