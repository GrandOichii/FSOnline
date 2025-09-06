function _Create()
    return FS.B.Loot('Choose a player. Prevent the next 1 damage they would take this turn.')
        .Target:Player()
        .Effect:Common(
            FS.C.Effect.PreventNextDamageToTargetPlayer(0, 1)
        )
    :Build()
end