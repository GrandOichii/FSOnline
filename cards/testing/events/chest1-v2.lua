-- status: implemented

function _Create()
    return FS.B.Event('Roll-\n1-2: Gain 1{cent}.\n3-4: Gain 3{cent}.\n5-6: Gain 6{cent}.')
        .Effect:Roll(
            FS.C.Effect.SwitchRoll(0, {
                [1] = FS.C.Effect.GainCoins(1),
                [2] = FS.C.Effect.GainCoins(1),
                [3] = FS.C.Effect.GainCoins(3),
                [4] = FS.C.Effect.GainCoins(3),
                [5] = FS.C.Effect.GainCoins(6),
                [6] = FS.C.Effect.GainCoins(6),
            })
        )
        -- Text here
        
    :Build()
end