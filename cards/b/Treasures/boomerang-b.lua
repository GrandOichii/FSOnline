-- status: not tested

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Steal a loot card at random from a player')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Target:Player()
                .Effect:Common(
                    FS.C.Effect.StealRandomLootCardsFromTarget(0, 1)
                )
            :Build()
        )
    :Build()
end