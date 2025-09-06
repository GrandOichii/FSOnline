function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('Discard a loot card', 'Gain 3{cent}')
                .Cost:Common(
                    FS.C.Cost.DiscardLoot(1)
                )
                .Effect:Common(
                    FS.C.Effect.GainCoins(3)
                )
            :Build()
        )
    :Build()
end