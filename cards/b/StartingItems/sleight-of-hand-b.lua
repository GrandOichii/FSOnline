-- status: implemented, requires testing

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Look at the top 3 cards of a deck. Put them back in any order.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Effect:Common(
                    FS.C.Effect.ReorderTop(3)
                )
            :Build()
        )        
        :Label(FS.Labels.Eternal)
    :Build()
end