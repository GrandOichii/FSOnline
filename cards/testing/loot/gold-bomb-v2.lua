-- status: implemented

function _Create()
    return FS.B.Loot()
        .Target:MonsterOrPlayer()
        .Effect:Common(
            FS.C.Effect.DamageToTarget(0, 3)
        )
    :Build()
end