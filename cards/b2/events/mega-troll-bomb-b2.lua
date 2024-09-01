-- status: not tested

function _Create()
    return FS.B.Event('Each player takes 2 damage!')
        .Effect:Common(
            FS.C.Effect.DamageToPlayer(2, FS.C.AllPlayers)
        )
    :Build()
end