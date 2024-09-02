-- status: implemented

function _Create()
    return FS.B.Loot('Deal 3 damage to a monster or player.')
        .Target:MonsterOrPlayer()
        .Effect:Common(
            FS.C.Effect.DamageToTarget(0, 3)
        )
    :Build()
end