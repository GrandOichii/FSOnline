-- status: implemented

function _Create()
    return FS.B.Monster()
        .Static:Common(
            'This takes no combat damage on attack rolls of 4 or 5.',
            FS.C.StateMod.TakeNoCombatDamageOnRollsForMonster({ 4, 5 })
        )
        :Reward(
            FS.B.Reward('+1 Treasure')
                .Effect:Common(
                    FS.C.Effect.GainTreasure(1)
                )
            :Build()
        )
    :Build()
end