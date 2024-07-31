-- status: implemented

function _Create()
    return FS.B.Item()
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time you take damage, put a counter on this.')
                .On:PlayerDamaged(function (me, player, args)
                    return player.Idx == args.Player.Idx
                end)
                .Effect:Common(
                    FS.C.Effect.PutGenericCountersOnMe(1)
                )
            :Build()
        )
        :ActivatedAbility(
            FS.B.ActivatedAbility('Remove a counter from this', 'Prevent the next 1 damage you would take this turn.')
                .Cost:Common(
                    FS.C.Cost.RemoveCounters(1)
                )
                .Effect:Common(
                    FS.C.Effect.PreventNextDamageToPlayer(1)
                )
            :Build()
        )
    :Build()
end