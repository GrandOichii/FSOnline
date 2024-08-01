function _Create()
    return FS.B.Item()
        :TriggeredAbility(
            FS.B.TriggeredAbility('At the start of your turn put a counter on this.')
                .On:TurnStart()
                .Effect:Common(
                    FS.C.Effect.PutGenericCountersOnMe(1)
                )
            :Build()
        )
        -- You have +1{health} for each counter on this.
        .Static:Raw(
            FS.ModLayers.PLAYER_MAX_HEALTH,
            function (me)
                me.Owner.Stats.State.Health = me.Owner.Stats.State.Health + GetCountersCount(me.IPID)
            end
        )
        :TriggeredAbility(
            FS.B.TriggeredAbility('At the end of your turn, if this has 3+ counters, remove all of them and loot 3.')
                .On:TurnEnd(function (me, player, args)
                    return player.Idx == args.playerIdx and GetCountersCount(me.IPID) >= 3
                end)
                .Effect:Custom(function (stackEffect)
                    local me = stackEffect.Card
                    RemoveCounters(me.IPID, GetCountersCount(me.IPID))

                    FS.C.Effect.Loot(3)(stackEffect)
                end)
            :Build()
        )
        :Label(FS.Labels.Eternal)
    :Build()
end