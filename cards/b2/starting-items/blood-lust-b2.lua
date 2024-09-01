-- status: not tested
-- TODO test

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Choose a player or monster. They gain +1{attack} till end of turn.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Target:MonsterOrPlayer()
                .Effect:Common(
                    FS.C.Effect.ModTargetAttackTEOT(0, 1)
                )
            :Build()
        )
        :TriggeredAbility(
            FS.B.TriggeredAbility('At the end of your turn, recharge this.')
                .On:ControllerTurnEnd()
                .Effect:Common(
                    FS.C.Effect.RechargeMe()
                )
            :Build()
        )
        :Label(FS.Labels.Eternal)
    :Build()
end