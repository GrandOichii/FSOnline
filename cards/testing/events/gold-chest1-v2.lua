-- status: not tested

function _Create()
    return FS.B.Event('Roll-\n1-2: Gain +1 Treasure.\n3-4: Loot 1.\n5-6: Loot 2.')
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
    :Build()
end