function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('Pay 3{cent}', 'Play an additional loot card this turn.')
                .Cost:Common(
                    FS.C.Cost.PayCoins(3)
                )
                .Effect:Common(
                    FS.C.Effect.AddLootPlay(1)
                )
            :Build()
        )
        :ActivatedAbility(
            FS.B.ActivatedAbility('Pay 4{cent}', 'Loot 1.')
                .Cost:Common(
                    FS.C.Cost.PayCoins(4)
                )
                .Effect:Common(
                    FS.C.Effect.Loot(1)
                )
            :Build()
        )
        -- TODO Pay 6{cent}: Deal 1 damage to a monster or player."
        :Label(FS.Labels.Eternal)
    :Build()
end