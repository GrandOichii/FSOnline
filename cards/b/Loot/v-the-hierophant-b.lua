-- status: not tested

function _Create()
    return FS.B.Loot('Prevent up to 2 damage dealt to a player or monster.')
        .Target:MonsterOrPlayer()
        .Effect:Common(
            FS.C.Effect.PreventNextDamageToTarget(0, 2)
        )
    :Build()
end