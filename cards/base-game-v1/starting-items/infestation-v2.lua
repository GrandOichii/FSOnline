-- status: not tested

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility()
                .Cost:Common(
                    FS.C.Cost:Tap()
                )
                .Effect:Common(
                    FS.C.Effect.Loot(2),
                    FS.C.Effect.Discard(1)
                )
                :Build()
        )
        :Label(FS.Labels.Eternal)
    :Build()
end