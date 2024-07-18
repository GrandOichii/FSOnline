-- status: not tested

function _Create()
    return FS.B.Item()
        -- TODO add back
        -- :ActivatedAbility(
        --     FS.B.ActivatedAbility('{T}', 'Loot 2, then discard 1 Loot card.')
        --         .Cost:Common(
        --             FS.C.Cost.Tap()
        --         )
        --         .Effect:Common(
        --             FS.C.Effect.Loot(2)
        --             -- TODO add back
        --             -- FS.C.Effect.Discard(1)
        --         )
        --         :Build()
        -- )
        -- TODO remove
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Gain 1{cent}.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Effect:Common(
                    FS.C.Effect.GainCoins(1)
                )
            :Build()
        )
        :Label(FS.Labels.Eternal)
    :Build()
end