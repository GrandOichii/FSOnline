-- status: implemented

function _Create()
    return FS.B.Loot('Loot 3.')
        .Effect:Common(
            FS.C.Effect.Loot(3)
        )
    :Build()
end