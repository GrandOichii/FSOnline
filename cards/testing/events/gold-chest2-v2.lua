-- status: not tested

function _Create()
    return FS.B.Event()
        .Effect:Roll(
            FS.C.Effect.SwitchRoll(0, {
                [1] = FS.C.Effect.GainTreasure(1),
                [2] = FS.C.Effect.GainTreasure(1),
                [3] = FS.C.Effect.GainCoins(5),
                [4] = FS.C.Effect.GainCoins(5),
                [5] = FS.C.Effect.GainCoins(7),
                [6] = FS.C.Effect.GainCoins(7),
            })
        )
        -- Text here
        
    :Build()
end