-- status: implemented

function _Create()
    return FS.B.Item()
        .Static:Common(
            'You may play an additional loot card on your turn.',
            FS.C.StateMod.ModMaxLootPlays(1)
        )
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time you take damage, you may recharge your Character card.')
                .On:ControllerDamaged()
                .Effect:Common(
                    FS.C.Effect.RechargeCharacter(true)
                )
            :Build()
        )
    :Build()
end