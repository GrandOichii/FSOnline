-- status: implemented

function _Create()
    return FS.B.Loot()
        .Static:Raw(
            FS.ModLayers.LOOT_AMOUNT,
            function (me)
                local pState = me.Owner.State
                pState.LootAmountModifiers:Add(function (player, amount, reason)
                    if reason.type ~= 'loot_phase' then
                        return amount
                    end
                    return amount + 1
                end)
            end
        )
        :TriggeredAbility(
            FS.B.TriggeredAbility('At the end of your turn, discard a loot card.')
                .On:TurnEnd(function (me, player, args)
                    return player.Idx == args.playerIdx and GetCountersCount(me.IPID) >= 3
                end)
                .Effect:Common(
                    FS.C.Effect.Discard(1)
                )
            :Build()
        )
        :Label(FS.Labels.Guppys)
    :Build()
end