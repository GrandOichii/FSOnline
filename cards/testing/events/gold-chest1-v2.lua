-- status: not tested

function _Create()
    return FS.B.Event()
        .Effect:Roll(
            FS.C.Effect.SwitchRoll(0, {
                [1] = FS.C.Effect.GainTreasure(1),
                [2] = FS.C.Effect.GainTreasure(1),
                [3] = FS.C.Effect.Loot(1),
                [4] = FS.C.Effect.Loot(1),
                [5] = FS.C.Effect.Loot(2),
                [6] = FS.C.Effect.Loot(2),
            })
        )
        -- Text here
        
    :Build()
end