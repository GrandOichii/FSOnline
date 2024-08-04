-- status: implemented

function _Create()
    return FS.B.Loot()
        .Target:MonsterOrPlayer()
        .Effect:Common(
            FS.C.Effect.DamageToTarget(0, 1)
        )
    :Build()
end