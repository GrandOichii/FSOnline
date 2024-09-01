-- status: implemented
-- TODO too low-level

function _Create()
    return FS.B.Item()
        -- !FIXME this does nothing when it enters play, only at the start of the next turn
        -- You may play an additional loot card on your turn.
        .Static:Raw(
            FS.ModLayers.MOD_MAX_LOOT_PLAYS,
            function (me)
                me.Owner.State.LootPlaysForTurn = me.Owner.State.LootPlaysForTurn + 1
            end
        )
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time you take damage, you may recharge your Character.')
                .On:PlayerDamaged(function (me, player, args)
                    return player.Idx == args.Player.Idx
                end)
                .Effect:Custom(function (stackEffect)
                    local character = GetPlayer(stackEffect.OwnerIdx).Character
                    local accept = FS.C.Choose.YesNo(stackEffect.OwnerIdx, 'Recharge '..character.LogName..'?')
                    if not accept then
                        return false
                    end
                    Recharge(character.IPID)
                    return true
                end)
            :Build()
        )
    :Build()
end