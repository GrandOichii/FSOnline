-- status: not tested

function _Create()
    return FS.B.Event()
        -- Text here
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