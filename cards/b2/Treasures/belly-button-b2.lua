-- status: implemented

function _Create()
    return FS.B.Loot()
        -- !FIXME this does nothing when it enters play, only at the start of the next turn
        .Static:Common(
            'You may play an additional loot card on your turn.',
            FS.C.StateMod.ModMaxLootPlays(1)
        )
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time you take damage, you may recharge your Character.')
                .On:ControllerDamaged()
                .Effect:Common(
                    FS.C.Effect.RechargeCharacter(true)
                )
            :Build()
        )
        :Trinket()
    :Build()
end