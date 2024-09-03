-- status: not tested

function _Create()
    return FS.B.Loot('Roll-\n1-2: Loot 1.\n3-4: Loot 3.\n5-6: Discard 1 loot card.')
        .Effect:Roll(FS.C.Effect.SwitchRoll(0,
        {
            [1] = FS.C.Effect.Loot(1),
            [2] = FS.C.Effect.Loot(1),
            [3] = FS.C.Effect.Loot(3),
            [4] = FS.C.Effect.Loot(3),
            [5] = FS.C.Effect.Discard(1),
            [6] = FS.C.Effect.Discard(1),
        }))
    :Build()
end