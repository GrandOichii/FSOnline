-- status: implemented

function _Create()
    -- Loot 3.
    return FS.B.Loot()
        .Effect:Common(
            FS.C.Effect.Loot(3)
        )
    :Build()
end