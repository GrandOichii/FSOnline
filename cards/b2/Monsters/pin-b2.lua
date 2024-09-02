-- status: implemented

function _Create()
    return FS.B.Monster()
        .Static:Common(
            'This takes no combat damage on attack rolls of 6.',
            FS.C.StateMod.TakeNoCombatDamageOnRollsForMonster({ 6 })
        )
        :Reward(
            FS.B.Reward('5{cent}')
                .Effect:Common(
                    FS.C.Effect.GainCoins(5)
                )
            :Build()
        )
    :Build()
end