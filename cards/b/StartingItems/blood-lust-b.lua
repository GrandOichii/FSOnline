-- status: not tested

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Add +1{attack} to a player or monster till the end of turn.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Target:MonsterOrPlayer()
                .Effect:Common(
                    FS.C.Effect.ModTargetAttackTEOT(0, 1)
                )
            :Build()
        )
        :Label(FS.Labels.Eternal)
    :Build()
end