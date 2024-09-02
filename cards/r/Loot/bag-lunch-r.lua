-- status: implemented
-- TODO too low-level

function _Create()
    return FS.B.Loot('')
        :Trinket()
        .Static:Raw(
            FS.ModLayers.ITEM_DESTRUCTION_REPLACEMENT_EFFECTS,
            'If this would be destroyed, if it has no counters on it, put a counter on it instead.',
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
            'You have +1{health} while this has a counter on it.',
            function (me)
                if GetCountersCount(me.IPID) == 0 then
                    return
                end
                me.Owner.Stats.State.Health = me.Owner.Stats.State.Health + 1
            end
        )
    :Build()
end