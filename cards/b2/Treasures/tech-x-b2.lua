-- status: not tested

-- !FIXME second ability doesn't work
function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Put a counter on this.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Effect:Common(
                    FS.C.Effect.PutGenericCountersOnMe(1)
                )
            :Build()
        )
        :ActivatedAbility(
            FS.B.ActivatedAbility('Remove 3 counters from this', 'Kill a player or monster.')
                .Cost:Common(
                    FS.C.Cost.RemoveCounters(3)
                )
                .Target:MonsterOrPlayer()
                .Effect:Common(
                    FS.C.Effect.KillTarget(0)
                )
            :Build()
        )

    :Build()
end