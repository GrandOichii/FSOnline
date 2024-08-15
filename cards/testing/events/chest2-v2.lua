-- status: not tested

function _Create()
    return FS.B.Event('Roll-\n1-2: Loot 1.\n3-4: Loot 2.\n5-6: Loot 3.')
        .Effect:Roll(
            FS.C.Effect.SwitchRoll(0, {
                [1] = FS.C.Effect.Loot(1),
                [2] = FS.C.Effect.Loot(1),
                [3] = FS.C.Effect.Loot(2),
                [4] = FS.C.Effect.Loot(2),
                [5] = FS.C.Effect.Loot(3),
                [6] = FS.C.Effect.Loot(3),
            })
        )
    :Build()
end