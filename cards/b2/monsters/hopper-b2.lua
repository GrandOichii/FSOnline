-- status: not tested

function _Create()
    return FS.B.Monster()
        .Static:Common(
            FS.C.StateMod.TakeNoCombatDamageOnRollsForMonster({ 6 })
        )
        :Reward(
            FS.B.Reward('3{cent}')
                .Effect:Common(
                    FS.C.Effect.GainCoins(3)
                )
            :Build()
        )
    :Build()
end