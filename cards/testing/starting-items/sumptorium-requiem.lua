-- status: implemented

function _Create()
    return FS.B.Item()
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time you take damage, put a counter on this.')
                .On:ControllerDamaged()
                .Effect:Common(
                    FS.C.Effect.PutGenericCountersOnMe(1)
                )
            :Build()
        )
        :ActivatedAbility(
            FS.B.ActivatedAbility('Remove 2 counters from this', 'A monster gains -1{evasion} till end of turn.')
                .Cost:Common(
                    FS.C.Cost.RemoveCounters(2)
                )
                .Target:Monster()
                .Effect:Common(
                    FS.C.Effect.ModTargetMonsterEvasionTEOT(0, -1)
                )
            :Build()
        )
        :Label(FS.Labels.Eternal)
    :Build()
end