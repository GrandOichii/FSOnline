-- status: implemented, requires testing

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', '')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Target:MonsterOrPlayer()
                .Effect:Common(
                    FS.C.Effect.PreventNextDamageToTarget(0, 1)
                )
            :Build()
        )        
        :Label(FS.Labels.Eternal)
    :Build()
end