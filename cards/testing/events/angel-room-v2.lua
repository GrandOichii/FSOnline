-- status: not tested

function _Create()
    return FS.B.Event('Roll-\n1: Gain +2 treasure.\n2-3: Gain +1 treasure.\n4-6: Loot 2.')
        .Effect:Roll(
            FS.C.Effect.SwitchRoll(0, {
                [1] = FS.C.Effect.GainTreasure(2),
                [2] = FS.C.Effect.GainTreasure(2),
                [3] = FS.C.Effect.GainTreasure(1),
                [4] = FS.C.Effect.GainTreasure(1),
                [5] = FS.C.Effect.Loot(2),
                [6] = FS.C.Effect.Loot(2),
            })
        )
        -- Text here
        
    :Build()
end