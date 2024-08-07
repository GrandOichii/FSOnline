-- status: implemented

function _Create()
    return FS.B.Event()
        -- Take 2 damage!
        .Effect:Common(
            FS.C.Effect.DamageToPlayer(2)
        )
    :Build()
end