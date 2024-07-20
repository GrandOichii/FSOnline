function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('Discard a loot card', 'Put a counter on this.')
                .Cost:Common(
                    FS.C.Cost.DiscardLoot(1)
                )
                .Effect:Common(
                    FS.C.Effect.PutGenericCountersOnMe(1)
                )
            :Build()
        )
        :ActivatedAbility(
            FS.B.ActivatedAbility('Remove 4 counters from this', 'Gain +1 treasure.')
                .Cost:Common(
                    FS.C.Cost.RemoveCounters(4)
                )
                .Effect:Common(
                    FS.C.Effect.GainTreasure(1)
                )
            :Build()
        )
        :Label(FS.Labels.Eternal)
    :Build()
end