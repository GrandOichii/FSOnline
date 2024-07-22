function _Create()
    return FS.B.Item()
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time a soul enters another player\'s control, put a counter on this. Then, if this has 3+ counters, it loses all abilities and becomes a soul.')
                .On:SoulEnter(function (me, player, args)
                    return args.Owner.Idx ~= me.Owner.Idx
                end)
                .Effect:Common(
                    FS.C.Effect.PutGenericCountersOnMe(1)
                )
                .Effect:Custom(function (stackEffect)
                    -- TODO check that the card is still here
                    local me = stackEffect.Card
                    if not IsPresent(me.IPID) then
                        return false
                    end

                    if GetCountersCount(me.IPID) < 3 then
                        -- TODO? return true
                        return false
                    end

                    RemoveFromPlay(me.IPID)
                    AddSoulCard(me.Owner.Idx, me.Card)
                    return true
                end)
            :Build()
        )
    :Build()
end