-- status: implemented
-- TODO too low-level

function _Create()
    return FS.B.Loot()
        :Trinket()
        .Static:Raw(
            FS.ModLayers.ITEM_DESTRUCTION_REPLACEMENT_EFFECTS,
            function (me)
                me.State.DestructionReplacementEffects:Add(function (card)
                    if not IsOwned(card) then
                        return false
                    end
                    if GetCountersCount(card.IPID) > 0 then
                        return false
                    end

                    PutGenericCounters(card.IPID, 1)
                    return true
                end)
            end
        )
        .Static:Raw(
            FS.ModLayers.PLAYER_MAX_HEALTH,
            function (me)
                if GetCountersCount(me.IPID) == 0 then
                    return
                end
                me.Owner.State.Stats.Health = me.Owner.State.Stats.Health + 1
            end
        )
    :Build()
end