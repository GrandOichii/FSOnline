-- status: implemented
-- TODO too low-level

function _Create()
    return FS.B.Item()
        -- !FIXME this does nothing when it enters play, only at the start of the next turn
        .Static:Raw(
            FS.ModLayers.MOD_MAX_LOOT_PLAYS,
            'You may play an additional loot card on your turn.',
            function (me)
                me.Owner.State.LootPlaysForTurn = me.Owner.State.LootPlaysForTurn + 1
            end
        )
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time you take damage, you may recharge your Character.')
                .On:ControllerDamaged()
                .Effect:Common(
                    FS.C.Effect.RechargeCharacter(true)
                )
            :Build()
        )
    :Build()
end