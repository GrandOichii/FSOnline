function _Create()
    return FS.B.Loot('Deal 1 damage to a Monster or player.')
        .Target:MonsterOrPlayer()
        .Effect:Common(
            FS.C.Effect.DamageToTarget(0, 1)
        )
    :Build()
end