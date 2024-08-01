-- status: implemented, TODO not stable

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Choose a player. The next time that player would die this turn, prevent it. If it\'s their turn, cancel everything that hasn\'t resolved and end it.')
                .Cost:Common(
                    FS.C.Cost:Tap()
                )
                .Target:Player()
                .Effect:Common(
                    FS.C.Effect.MantleTargetPlayer(0)
                )
            :Build()
        )
        :TriggeredAbility(
            FS.B.TriggeredAbility('At the end of your turn, recharge this.')
                .On:TurnEnd()
                .Effect:Common(
                    FS.C.Effect.RechargeMe()
                )
            :Build()
        )
        :Label(FS.Labels.Eternal)
    :Build()
end